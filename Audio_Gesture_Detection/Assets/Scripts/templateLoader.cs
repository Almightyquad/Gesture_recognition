using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class templateLoader : MonoBehaviour {

    struct RecordedData
    {
        public List<Vector3> dataVector;

        public void initialize()
        {
            dataVector = new List<Vector3>();
        }

        public void addData(Vector3 dataToAdd)
        {
            dataVector.Add(dataToAdd);
        }
        //This needs to have proper error management if the data is too small for the template.
        public List<Vector3> computeLikeness(List<Vector3> templateData)
        {
            List<Vector3> score = new List<Vector3>();

            if(dataVector.Count == templateData.Count)
            {
                for (int i = 0; i < dataVector.Count; i++)
                {
                    score.Add(dataVector[i] - templateData[i]);
                }
            }
            

            return score;
        }
            
    }

    ReadScript readScript;
    List<GestureDetection.ObjectInfo> headObjects;
    List<GestureDetection.ObjectInfo> leftControllerObjects;
    List<GestureDetection.ObjectInfo> rightControllerObjects;
    List<string> templateNames;

    List<GameObject> headGameObjects;
    List<GameObject> leftControllerGameObjects;
    List<GameObject> rightControllerGameObjects;

    List<List<Pair<List<Vector3>, int>>> headExpandedTemplate;
    List<List<Pair<List<Vector3>, int>>> leftExpandedTemplate;
    List<List<Pair<List<Vector3>, int>>> rightExpandedTemplate;

    //List<List<Vector3>> recordedData;
    List<RecordedData> headData;
    List<RecordedData> leftData;
    List<RecordedData> rightData;
    int currentNumberOfCollections;
    int biggestTemplate;

    public float threshold;

    //List<Vector3> posVectorList;
    //List<Vector3> rotVectorList;
    //List<float> timesList;
    //string timestamp;
    public string pathInAssets;
    // Use this for initialization
    void Start () {
        biggestTemplate = 0;

        readScript = new ReadScript();
        headObjects = new List<GestureDetection.ObjectInfo>();
        leftControllerObjects = new List<GestureDetection.ObjectInfo>();
        rightControllerObjects = new List<GestureDetection.ObjectInfo>();
        templateNames = new List<string>();
        headData = new List<RecordedData>();
        leftData = new List<RecordedData>();
        rightData = new List<RecordedData>();
        headExpandedTemplate = new List<List<Pair<List<Vector3>, int>>>();
        leftExpandedTemplate = new List<List<Pair<List<Vector3>, int>>>();
        rightExpandedTemplate = new List<List<Pair<List<Vector3>, int>>>();

        readTemplates();

        generateDataset();

        deleteCollections(ref currentNumberOfCollections);

        generateTemplate();
        
        for (int i = 0; i < headObjects.Count; i++)
        {
            //Don't know if I need this, check later
            if(headObjects[i].resetRotPos.Count > biggestTemplate)
            {
                biggestTemplate = headObjects[i].resetRotPos.Count;
            }
            headExpandedTemplate.Add(new List<Pair<List<Vector3>, int>>());
            leftExpandedTemplate.Add(new List<Pair<List<Vector3>, int>>());
            rightExpandedTemplate.Add(new List<Pair<List<Vector3>, int>>());
        }

        expandTemplates();
        for (int i = 0; i < headExpandedTemplate.Count; i++)
        {
            if (headExpandedTemplate[i][headExpandedTemplate[i].Count - 1].second > biggestTemplate)
            {
                biggestTemplate = headExpandedTemplate[i][headExpandedTemplate[i].Count - 1].second;
            }
        }
            //recordedData = new List<List<Vector3>>();
    }

    //This is convoluted as fuck
    public List<string> checkTemplates(ref List<float> score)
    {
        List<float> headScore = new List<float>();
        List<float> leftScore = new List<float>();
        List<float> rightScore = new List<float>();
        List<string> templateNames = new List<string>();
        List<int> templateHits = new List<int>();
        List<List<Vector3>> headLikeness = new List<List<Vector3>>();
        List<List<Vector3>> leftLikeness = new List<List<Vector3>>();
        List<List<Vector3>> rightLikeness = new List<List<Vector3>>();

        int size = 0;
        for (int i = 0; i < headExpandedTemplate.Count; i++)
        {
            for (int j = 0; j < headData.Count; j++)
            {
                size = headData[j].dataVector.Count;
                if(size > headExpandedTemplate[i][0].second && size < headExpandedTemplate[i][headExpandedTemplate.Count-1].second)
                {
                    for (int k = 0; k < headExpandedTemplate[i].Count; k++)
                    {
                        //This if statement seems expensive
                        if (size == headExpandedTemplate[i][k].second)
                        {
                            templateHits.Add(i);
                            headLikeness.Add(headData[j].computeLikeness(headExpandedTemplate[i][k].first));
                            leftLikeness.Add(leftData[j].computeLikeness(leftExpandedTemplate[i][k].first));
                            rightLikeness.Add(rightData[j].computeLikeness(rightExpandedTemplate[i][k].first));
                        }
                    }
                }
            }
        }



        for (int i = 0; i < headLikeness.Count; i++)
        {
            Vector3 headTempVec3 = Vector3.zero;
            Vector3 leftTempVec3 = Vector3.zero;
            Vector3 rightTempVec3 = Vector3.zero;
            headScore.Add(new float());
            for(int j = 0; j < headLikeness[i].Count; j++)
            {
                headTempVec3 = headLikeness[i][j] + headTempVec3;
                leftTempVec3 = leftLikeness[i][j] + leftTempVec3;
                rightTempVec3 = rightLikeness[i][j] + rightTempVec3;
            }
            headScore[i] = headTempVec3.x + headTempVec3.y + headTempVec3.z;
            leftScore[i] = leftTempVec3.x + leftTempVec3.y + leftTempVec3.z;
            rightScore[i] = rightTempVec3.x + rightTempVec3.y + rightTempVec3.z;

            //This thing won't tell me shit
            //if (headScore[i] < threshold)
            //{
            //    templateNames.Add(this.templateNames[templateHits[i]]);
            //}

            //Checking only right for now because that is the only controller used for the current gestures.
            if (rightScore[i] < threshold)
            {
                templateNames.Add(this.templateNames[templateHits[i]]);
            }
        }
        score = rightScore;

        return templateNames;
    }

    public void addData(Vector3 head, Vector3 leftController, Vector3 rightController)
    {
        headData.Add(new RecordedData());
        leftData.Add(new RecordedData());
        rightData.Add(new RecordedData());
        List<int> indicesToRemove = new List<int>();
        for(int i = 0; i < headData.Count; i++)
        {
            headData[i].addData(head);
            leftData[i].addData(leftController);
            rightData[i].addData(rightController);

            if(headData[i].dataVector.Count > biggestTemplate)
            {
                indicesToRemove.Add(i);
            }
        }
        for (int i = indicesToRemove.Count - 1; i < 0; i--)
        {
            headData.RemoveAt(indicesToRemove[i]);
            leftData.RemoveAt(indicesToRemove[i]);
            rightData.RemoveAt(indicesToRemove[i]);
        }


    }

    void expandTemplates()
    {
        int originalsize = 0;
        for (int i = 0; i < headExpandedTemplate.Count; i++)
        {
            originalsize = headObjects[i].resetRotPos.Count;
            if (originalsize < 20)
            {
                Debug.LogError("Templates must be length 20");
            }
            for (int j = originalsize - 20; j < originalsize + 20; j++)
            {
                headExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(Interpolator.interpolate(headObjects[i].resetRotPos, j), j));
                leftExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(Interpolator.interpolate(leftControllerObjects[i].resetRotPos, j), j));
                rightExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(Interpolator.interpolate(rightControllerObjects[i].resetRotPos, j), j));
            }
        }
    }

    //Not strictly needed.
    void addTemplate(ref List<List<Pair<List<Vector3>, int>>> expandedTemplate, int index, List<Vector3> template, int size)
    {
        expandedTemplate[index].Add(new Pair<List<Vector3>, int>(template, size));
    }

    void readTemplates()
    {
        List<Vector3> posVectorList = new List<Vector3>();
        List<Vector3> rotVectorList = new List<Vector3>();
        List<float> timesList = new List<float>();
        string timeStamp = "";

        string path = Application.dataPath + pathInAssets;
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + pathInAssets);
        FileInfo[] info = dir.GetFiles("*.txt");
        int first = info.Length / 3;
        int second = info.Length / 3 + first;
        int third = info.Length / 3 + second;
        for (int i = 0; i < first; i++)
        {
            templateNames.Add(info[i].Name);
            assignData(ref headObjects, i, path, info[i], ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
            assignData(ref leftControllerObjects, i, path, info[i + first], ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
            assignData(ref rightControllerObjects, i, path, info[i + second], ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
        }
        headGameObjects = createGameObjects(headObjects.Count, "head");
        leftControllerGameObjects = createGameObjects(leftControllerObjects.Count, "left");
        rightControllerGameObjects = createGameObjects(rightControllerObjects.Count, "right");
        currentNumberOfCollections = createCollections(headGameObjects, leftControllerGameObjects, rightControllerGameObjects);

    }

    //This function does not generate the templates
    void generateTemplate()
    {
        for (int i = 0; i < headObjects.Count; i++)
        {
            normalizeVectors(ref headObjects[i].resetRotPos);
            normalizeVectors(ref leftControllerObjects[i].resetRotPos);
            normalizeVectors(ref rightControllerObjects[i].resetRotPos);
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
    
    void normalizeVectors(ref List<Vector3> posVectorList)
    {
        for (int i = 0; i < posVectorList.Count; i++)
        {
            posVectorList[i] = posVectorList[i].normalized;
        }
    }

    void assignData(ref List<GestureDetection.ObjectInfo> obj, int iterator, string path, FileInfo info, ref List<Vector3> posVectorList, ref List<Vector3> rotVectorList, ref List<float> timesList, ref string timeStamp)
    {
        readScript.readFile(path + info.Name, ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
        obj.Add(new GestureDetection.ObjectInfo());
        obj[iterator].posVectorList = posVectorList;
        obj[iterator].rotVectorList = rotVectorList;
        obj[iterator].timesList = timesList;
        obj[iterator].timeStamp = timeStamp;
    }

    void generateDataset()
    {
        for (int i = 0; i < headObjects.Count; i++)
        {
            for (int j = 0; j < headObjects[i].posVectorList.Count; j++)
            {
                headGameObjects[i].transform.position = headObjects[i].posVectorList[j];
                headGameObjects[i].transform.eulerAngles = headObjects[i].rotVectorList[j];

                leftControllerGameObjects[i].transform.position = leftControllerObjects[i].posVectorList[j];
                leftControllerGameObjects[i].transform.eulerAngles = leftControllerObjects[i].rotVectorList[j];

                rightControllerGameObjects[i].transform.position = rightControllerObjects[i].posVectorList[j];
                rightControllerGameObjects[i].transform.eulerAngles = rightControllerObjects[i].rotVectorList[j];
                
                resetRotation(headGameObjects[i], rightControllerGameObjects[i], leftControllerGameObjects[i]);
                resetPositionToOrigin(headGameObjects[i], rightControllerGameObjects[i], leftControllerGameObjects[i]);

                //Headobjects reset rotation position is not strictly needed
                headObjects[i].resetRotPos.Add(headGameObjects[i].transform.position);
                leftControllerObjects[i].resetRotPos.Add(leftControllerGameObjects[i].transform.position);
                rightControllerObjects[i].resetRotPos.Add(rightControllerGameObjects[i].transform.position);

                //Directions, probably needed
                leftControllerObjects[i].directions.Add(leftControllerObjects[i].resetRotPos[j] - headObjects[i].resetRotPos[j]);
                rightControllerObjects[i].directions.Add(rightControllerObjects[i].resetRotPos[j] - headObjects[i].resetRotPos[j]);

                //Distances, it's stupid to add in the calculations manually right?!
                leftControllerObjects[i].distances.Add(Vector3.Distance(headObjects[i].resetRotPos[j], leftControllerObjects[i].resetRotPos[j]));
                rightControllerObjects[i].distances.Add(Vector3.Distance(headObjects[i].resetRotPos[j], rightControllerObjects[i].resetRotPos[j]));
            }
        }
    }

    //Creates collections for lists of game objects for less clutter and easier deletion.
    int createCollections(List<GameObject> heads, List<GameObject> lefts, List<GameObject> rights)
    {
        for (int i = 0; i < heads.Count; i++)
        {
            GameObject parent = new GameObject();
            parent.name = "TemplateCollection" + i;
            heads[i].transform.SetParent(parent.transform);
            lefts[i].transform.SetParent(parent.transform);
            rights[i].transform.SetParent(parent.transform);
        }
        return heads.Count;
    }

    //Deletes collections of game objects created and sets the number sent in to 0.
    void deleteCollections(ref int numberToDestroy)
    {
        for (int i = 0; i < numberToDestroy; i++)
        {
            Destroy(GameObject.Find("TemplateCollection" + i));
        }
        numberToDestroy = 0;
    }

    //Creates gameobjects for calculations
    public List<GameObject> createGameObjects(int numberToCreate, string typeOfObject)
    {
        List<GameObject> gameObjects = new List<GameObject>();

        for (int i = 0; i < numberToCreate; i++)
        {
            gameObjects.Add(new GameObject(typeOfObject + i));
        }

        return gameObjects;
    }
    
    //Resets rotation on the heads Y axis
    public void resetRotation(GameObject headData, GameObject rightControllerData, GameObject leftControllerData)
    {
        rightControllerData.transform.RotateAround(headData.transform.position, Vector3.up, -headData.transform.eulerAngles.y);
        leftControllerData.transform.RotateAround(headData.transform.position, Vector3.up, -headData.transform.eulerAngles.y);
        headData.transform.eulerAngles = new Vector3(headData.transform.eulerAngles.x, 0f, headData.transform.eulerAngles.z);
    }

    //Sets the heads position to origin and the controllers are moved according to the distance that it the head is from the origin
    public void resetPositionToOrigin(GameObject headData, GameObject rightControllerData, GameObject leftControllerData)
    {
        rightControllerData.transform.position = rightControllerData.transform.position - headData.transform.position;
        leftControllerData.transform.position = leftControllerData.transform.position - headData.transform.position;
        headData.transform.position = Vector3.zero;

    }
}
