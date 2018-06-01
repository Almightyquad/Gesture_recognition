using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public class GestureDetection : MonoBehaviour {

    public class ObjectInfo
    {
        public List<Vector3> posVectorList;
        public List<Vector3> rotVectorList;
        public List<float> timesList;
        public string timeStamp;
        public List<Vector3> resetRotPos;
        public List<Vector3> directions;
        public List<float> distances;
        public List<Vector3> positiveVec;

        public ObjectInfo()
        {
            posVectorList = new List<Vector3>();
            List<Vector3> rotVectorList = new List<Vector3>();
            List<float> timesList = new List<float>();
            string timeStamp = "";
            resetRotPos = new List<Vector3>();
            directions = new List<Vector3>();
            distances = new List<float>();
            positiveVec = new List<Vector3>();
        }

    }

    int currentNumberOfCollections;

    public string PathInAssets;
    ReadScript readScript;
    List<ObjectInfo> headObjects;
    List<ObjectInfo> leftControllerObjects;
    List<ObjectInfo> rightControllerObjects;

    List<GameObject> headGameObjects;
    List<GameObject> leftControllerGameObjects;
    List<GameObject> rightControllerGameObjects;

    GameObject headGameObject;
    GameObject leftControllerGameObject;
    GameObject rightControllerGameObject;


    Transform cameraEye;
    Transform controllerLeft;
    Transform controllerRight;

    List<List<float>> scores;
    bool triggerClicked = false;
    bool recording = false;
    IOScript ioWriter;

    int recordingIndex = 0;
    //Need this for button interaction, should probably find some other way of doing it.
    GameObject actualRightControllerObject;

    public bool canRecord = false;

    private void Awake()
    {

        actualRightControllerObject = GameObject.Find("RightController");
    }

    // Use this for initialization
    void Start() {
        readScript = new ReadScript();
        headObjects = new List<ObjectInfo>();
        leftControllerObjects = new List<ObjectInfo>();
        rightControllerObjects = new List<ObjectInfo>();

        GameObject parent = new GameObject("Tranformer");
        headGameObject = new GameObject("Head");
        leftControllerGameObject = new GameObject("ControllerLeft");
        rightControllerGameObject = new GameObject("ControllerRight");

        //Just for sanity in the editor
        headGameObject.transform.SetParent(parent.transform);
        leftControllerGameObject.transform.SetParent(parent.transform);
        rightControllerGameObject.transform.SetParent(parent.transform);

        GameObject cameraRig = GameObject.Find("[CameraRig]");
        cameraEye = cameraRig.transform.FindChild("Camera (eye)");
        controllerLeft = cameraRig.transform.FindChild("Controller (left)");
        controllerRight = cameraRig.transform.FindChild("Controller (right)");

        scores = new List<List<float>>();
        ioWriter = new IOScript();

        //readData();

        //generateDataset();

        //deleteCollections(ref currentNumberOfCollections);
    }
	
	// Update is called once per frame
	void Update () {
        if (canRecord)
        {
            var controllerEvents = actualRightControllerObject.GetComponent<VRTK_ControllerEvents>();
            //if (controllerEvents.IsButtonPressed(VRTK_ControllerEvents.ButtonAlias.Trigger_Click) || Input.GetKeyUp(KeyCode.R))
            if (Input.GetKeyUp(KeyCode.R))
            {
                triggerClicked = true;
            }
            else if (triggerClicked)
            {
                triggerClicked = false;
                if (recording)
                {
                    Debug.Log("Stopped Recording");
                    stopRecording();
                }
                else if (!recording)
                {
                    Debug.Log("Started Recording");
                    startRecording();
                }
            }
        }
        



        var templater = this.gameObject.GetComponent<templateLoader>();

        headGameObject.transform.position = cameraEye.position;
        leftControllerGameObject.transform.position = controllerLeft.position;
        rightControllerGameObject.transform.position = controllerRight.position;

        headGameObject.transform.rotation = cameraEye.rotation;
        leftControllerGameObject.transform.rotation = controllerLeft.rotation;
        rightControllerGameObject.transform.rotation = controllerRight.rotation;

        

        resetRotation(headGameObject, rightControllerGameObject, leftControllerGameObject);
        resetPositionToOrigin(headGameObject, rightControllerGameObject, leftControllerGameObject);

        //Debug.Log(rightControllerGameObject.transform.position.magnitude);
        //Debug.Log(rightControllerGameObject.transform.position);
        //Debug.Log(rightControllerGameObject.transform.position.normalized + "  POS");
        Vector3 tempVec3 = new Vector3(1f, 1f, 1f);
        //Debug.Log((rightControllerGameObject.transform.position.normalized + tempVec3) / 2);


        //headGameObject.transform.position = (headGameObject.transform.position.normalized + tempVec3) / 2;
        //leftControllerGameObject.transform.position = (leftControllerGameObject.transform.position.normalized + tempVec3) / 2;
        //rightControllerGameObject.transform.position = (rightControllerGameObject.transform.position.normalized + tempVec3) / 2;


        headGameObject.transform.position = (headGameObject.transform.position + tempVec3) / 2;
        leftControllerGameObject.transform.position = (leftControllerGameObject.transform.position + tempVec3) / 2;
        rightControllerGameObject.transform.position = (rightControllerGameObject.transform.position + tempVec3) / 2;

        templater.addData(headGameObject.transform.position, leftControllerGameObject.transform.position, rightControllerGameObject.transform.position);
        var score = new List<List<float>>();
        //Debug.Log("Running");
        var templateNames = templater.checkTemplates(ref score);
        for(int i = 0; i < score.Count; i++)
        {
            //Debug.Log("Score: " + s);
            
        }
        if (recording)
        {
            foreach(var s in score)
            {
                //Debug.Log("Score: " + s);

                scores.Add(s);
            }
        }
        //Debug.Log("Filler");
        foreach (var t in templateNames)
        {
            Debug.Log(t);

        }
        
    }

    void readData()
    {
        List<Vector3> posVectorList = new List<Vector3>();
        List<Vector3> rotVectorList = new List<Vector3>();
        List<float> timesList = new List<float>();
        string timeStamp = "";

        string path = Application.dataPath + PathInAssets;
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + PathInAssets);
        FileInfo[] info = dir.GetFiles("*.txt");
        int first = info.Length / 3;
        int second = info.Length / 3 + first;
        int third = info.Length / 3 + second;
        for (int i = 0; i < first; i++)
        {
            assignData(ref headObjects, i, path, info[i], ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
            assignData(ref leftControllerObjects, i, path, info[i + first], ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
            assignData(ref rightControllerObjects, i, path, info[i + second], ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
        }
        headGameObjects = createGameObjects(headObjects.Count, "head");
        leftControllerGameObjects = createGameObjects(leftControllerObjects.Count, "left");
        rightControllerGameObjects = createGameObjects(rightControllerObjects.Count, "right");
        currentNumberOfCollections = createCollections(headGameObjects, leftControllerGameObjects, rightControllerGameObjects);
    }

    void assignData(ref List<ObjectInfo> obj, int iterator, string path, FileInfo info, ref List<Vector3> posVectorList, ref List<Vector3> rotVectorList, ref List<float> timesList, ref string timeStamp)
    {
        readScript.readFile(path + info.Name, ref posVectorList, ref rotVectorList, ref timesList, ref timeStamp);
        obj.Add(new ObjectInfo());
        obj[iterator].posVectorList = posVectorList;
        obj[iterator].rotVectorList = rotVectorList;
        obj[iterator].timesList = timesList;
        obj[iterator].timeStamp = timeStamp;
    }

    //Creates gameobjects for calculations
    public List<GameObject> createGameObjects(int numberToCreate, string typeOfObject)
    {
        List<GameObject> gameObjects = new List<GameObject>();

        for(int i = 0; i < numberToCreate; i++)
        {
            gameObjects.Add(new GameObject(typeOfObject + i));
        }

        return gameObjects;
    }

    //Creates collections for lists of game objects for less clutter and easier deletion.
    public int createCollections(List<GameObject> heads, List<GameObject> lefts, List<GameObject> rights)
    {
        for (int i = 0; i < heads.Count; i++)
        {
            GameObject parent = new GameObject();
            parent.name = "Collection" + i;
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
            Destroy(GameObject.Find("Collection" + i));
        }
        numberToDestroy = 0;
    }


    public void resetRotation(GameObject headData, GameObject rightControllerData, GameObject leftControllerData)
    {
        rightControllerData.transform.RotateAround(headData.transform.position, Vector3.up, -headData.transform.eulerAngles.y);
        leftControllerData.transform.RotateAround(headData.transform.position, Vector3.up, -headData.transform.eulerAngles.y);
        headData.transform.eulerAngles = new Vector3(headData.transform.eulerAngles.x, 0f, headData.transform.eulerAngles.z);
    }

    public void resetPositionToOrigin(GameObject headData, GameObject rightControllerData, GameObject leftControllerData)
    {
        rightControllerData.transform.position = rightControllerData.transform.position - headData.transform.position;
        leftControllerData.transform.position = leftControllerData.transform.position - headData.transform.position;
        headData.transform.position = Vector3.zero;

    }

    void generateDataset()
    {
        for(int i = 0; i < headObjects.Count; i++)
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

    void simplifyData(GameObject headData, GameObject rightControllerData, GameObject leftControllerData)
    {
        //Direction
        Vector3 directionToRightController = rightControllerData.transform.position - headData.transform.position;
        Vector3 directionToLeftController = leftControllerData.transform.position - headData.transform.position;

        //Distance
        float rightDistance = Mathf.Pow((headData.transform.position.x - directionToRightController.x), 2) + Mathf.Pow((headData.transform.position.y - directionToRightController.y), 2) + Mathf.Pow((headData.transform.position.z - directionToRightController.z), 2);
        float leftDistance = Mathf.Pow((headData.transform.position.x - directionToLeftController.x), 2) + Mathf.Pow((headData.transform.position.y - directionToLeftController.y), 2) + Mathf.Pow((headData.transform.position.z - directionToLeftController.z), 2);

        //Angles?
        Vector3 rightAngle = Quaternion.LookRotation(directionToRightController).eulerAngles;
        Vector3 leftAngle = Quaternion.LookRotation(directionToLeftController).eulerAngles;

    }


    void stopRecording()
    {
        recording = false;
        recordingIndex++;

        var templater = this.gameObject.GetComponent<templateLoader>();
        ioWriter.WriteToFileAsCSV("template", recordingIndex, scores, templater.getTemplateNames());

        scores.Clear();
        Debug.Log("Written");
    }

    void startRecording()
    {
        recording = true;
    }
}
