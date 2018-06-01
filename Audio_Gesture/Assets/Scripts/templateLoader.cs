using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class templateLoader : MonoBehaviour {

    class RecordedData
    {
        public List<Vector3> dataVector;

        public RecordedData()
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

            //Debug.Log(templateData.Count + " " + dataVector.Count);
            for (int i = 0; i < dataVector.Count; i++)
            {
                score.Add(powVec3((dataVector[i] - templateData[i]), 2));
            }
            

            return score;
        }

        //This weighting makes no sense. it needs to reduce the score, not increase it.
        public List<Vector3> computeGaussianWeightedLikeness(List<Vector3> templateData)
        {
            List<Vector3> score = new List<Vector3>();
            List<float> gauss = new List<float>(dataVector.Count);
            int halfPoint = Mathf.RoundToInt(dataVector.Count);
            int iterator = 1;
            gauss[halfPoint] = 2;
            for (int i = halfPoint - 1; i >= 0; i++)
            {
                gauss[i] = 1 + Mathf.Pow((1 / iterator), 2);
                iterator++;
            }
            iterator = 1;
            for (int i = halfPoint + 1; i < dataVector.Count; i++)
            {
                gauss[i] = 1 + Mathf.Pow((1 / iterator), 2);
                iterator++;
            }
            for (int i = 0; i < dataVector.Count; i++)
            {
                score.Add(powVec3((dataVector[i] - templateData[i]), 2) / gauss[i]);
            }

            return score;
        }

        Vector3 powVec3(Vector3 toPow, float pow)
        {
            toPow.x = Mathf.Pow(toPow.x, 2);
            toPow.y = Mathf.Pow(toPow.y, 2);
            toPow.z = Mathf.Pow(toPow.z, 2);
            return toPow;
        }

    }

    ReadScript readScript;
    List<GestureDetection.ObjectInfo> headObjects;
    List<GestureDetection.ObjectInfo> leftControllerObjects;
    List<GestureDetection.ObjectInfo> rightControllerObjects;
    List<string> templateNames;

    public List<string> getTemplateNames()
    {
        return templateNames;
    }

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

    public int expandTemplatesBy = 1;

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

        //expandTemplates();

        //Add these three lines, remove expandTemplates() above and only have 1 template in the template folder to only have 1 template
        //headExpandedTemplate[0].Add(new Pair<List<Vector3>, int>(headObjects[0].positiveVec, headObjects[0].positiveVec.Count));
        //leftExpandedTemplate[0].Add(new Pair<List<Vector3>, int>(leftControllerObjects[0].positiveVec, leftControllerObjects[0].positiveVec.Count));
        //rightExpandedTemplate[0].Add(new Pair<List<Vector3>, int>(rightControllerObjects[0].positiveVec, rightControllerObjects[0].positiveVec.Count));

        for (int i = 0; i < headExpandedTemplate.Count; i++)
        {
            headExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(headObjects[i].positiveVec, headObjects[i].positiveVec.Count));
            leftExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(leftControllerObjects[i].positiveVec, leftControllerObjects[i].positiveVec.Count));
            rightExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(rightControllerObjects[i].positiveVec, rightControllerObjects[i].positiveVec.Count));
        }

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
    public List<string> checkTemplates(ref List<List<float>> score)
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
        //Debug.Log(headData.Count);
        for (int i = 0; i < headExpandedTemplate.Count; i++)
        {
            for (int j = 0; j < headData.Count; j++)
            {
                size = headData[j].dataVector.Count;
                for (int k = 0; k < headExpandedTemplate[i].Count; k++)
                {
                    //This if statement seems expensive
                    if (size == headExpandedTemplate[i][k].first.Count)
                    {
                        //Debug.Log(" SIZES MATTER: " + size  +" "+ headExpandedTemplate[i][k].first.Count);
                        templateHits.Add(i);
                        headLikeness.Add(headData[j].computeLikeness(headExpandedTemplate[i][k].first));
                        //Debug.Log(" SIZES MATTER: " + size + " " + leftExpandedTemplate[i][k].second + " " + rightExpandedTemplate[i][k].second);
                        leftLikeness.Add(leftData[j].computeLikeness(leftExpandedTemplate[i][k].first));
                        rightLikeness.Add(rightData[j].computeLikeness(rightExpandedTemplate[i][k].first));
                    }
                }
            }
        }


        //Debug.Log(headLikeness.Count);
        for (int i = 0; i < headLikeness.Count; i++)
        {
            Vector3 headTempVec3 = Vector3.zero;
            Vector3 leftTempVec3 = Vector3.zero;
            Vector3 rightTempVec3 = Vector3.zero;
            headScore.Add(new float());
            leftScore.Add(new float());
            rightScore.Add(new float());
            //Debug.Log(headLikeness[i].Count + " HEADCOUNT");
            for (int j = 0; j < headLikeness[i].Count; j++)
            {
                headTempVec3 = headLikeness[i][j] + headTempVec3;
                leftTempVec3 = leftLikeness[i][j] + leftTempVec3;
                rightTempVec3 = rightLikeness[i][j] + rightTempVec3;
            }
            headScore[i] = headTempVec3.x + headTempVec3.y + headTempVec3.z;
            leftScore[i] = leftTempVec3.x + leftTempVec3.y + leftTempVec3.z;
            rightScore[i] = rightTempVec3.x + rightTempVec3.y + rightTempVec3.z;


            //headScore[i] = Mathf.Pow(headScore[i], 2);
            //leftScore[i] = Mathf.Pow(leftScore[i], 2);
            //rightScore[i] = Mathf.Pow(rightScore[i], 2);

            //Debug.Log(rightTempVec3.x + " " + rightTempVec3.y + " " + rightTempVec3.z);
            //This thing won't tell me shit
            //if (headScore[i] < threshold)
            //{
            //    templateNames.Add(this.templateNames[templateHits[i]]);
            //}

            //Checking only right for now because that is the only controller used for the current gestures.
            if (rightScore[i] < threshold && rightScore[i] > (threshold * (-1)))
            {
                templateNames.Add(this.templateNames[templateHits[i]]);
            }
            score.Add(rightScore);
        }
        return templateNames;
    }

    Vector3 powVec3(Vector3 toPow, float pow)
    {
        toPow.x = Mathf.Pow(toPow.x, 2);
        toPow.y = Mathf.Pow(toPow.y, 2);
        toPow.z = Mathf.Pow(toPow.z, 2);
        return toPow;
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
        for (int i = indicesToRemove.Count - 1; i >= 0; i--)
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
            originalsize = headObjects[i].positiveVec.Count;
            if (originalsize < 20)
            {
                Debug.LogError("Templates must be at least length 20");
            }

            //Todo, fix the sillyness that is the fucking size.
            for (int j = originalsize - expandTemplatesBy; j < originalsize + expandTemplatesBy; j++)
            {
                headExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(Interpolator.interpolate(headObjects[i].positiveVec, j), j));
                leftExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(Interpolator.interpolate(leftControllerObjects[i].positiveVec, j), j));
                rightExpandedTemplate[i].Add(new Pair<List<Vector3>, int>(Interpolator.interpolate(rightControllerObjects[i].positiveVec, j), j));
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

        string path = Application.streamingAssetsPath + pathInAssets;
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + pathInAssets);
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
            //normalizeVectors(ref headObjects[i].resetRotPos);
            //normalizeVectors(ref leftControllerObjects[i].resetRotPos);
            //normalizeVectors(ref rightControllerObjects[i].resetRotPos);

            makeThingsPositive(ref headObjects[i].positiveVec, headObjects[i].resetRotPos);
            makeThingsPositive(ref leftControllerObjects[i].positiveVec, leftControllerObjects[i].resetRotPos);
            makeThingsPositive(ref rightControllerObjects[i].positiveVec, rightControllerObjects[i].resetRotPos);
            
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

    void makeThingsPositive(ref List<Vector3> positiveVec, List<Vector3> positionsToMakePositive)
    {
        Vector3 tempVec3 = new Vector3(1f, 1f, 1f);

        for (int i = 0; i < positionsToMakePositive.Count; i++)
        {
            positiveVec.Add((positionsToMakePositive[i] + tempVec3) / 2);
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
