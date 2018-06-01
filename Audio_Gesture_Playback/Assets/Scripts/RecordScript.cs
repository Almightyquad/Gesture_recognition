using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordScript : MonoBehaviour {
    IOScript ioWriter;

    Transform head;
    Transform controllerLeft;
    Transform controllerRight;

    List<Vector3> headPositions;
    List<Vector3> controllerLeftPositions;
    List<Vector3> controllerRightPositions;

    List<Vector3> headRotations;
    List<Vector3> controllerLeftRotations;
    List<Vector3> controllerRightRotations;

    //Only need 1 time since I sample all of the objects at the same time
    List<float> times;
    System.DateTime startTime;
    float timer;
    bool recording;
    int recordingIndex;
    // Use this for initialization
    void Start () {
        GameObject cameraRig = GameObject.Find("[CameraRig]");
        head = cameraRig.transform.FindChild("Camera (eye)");
        controllerLeft = cameraRig.transform.FindChild("Controller (left)");
        controllerRight = cameraRig.transform.FindChild("Controller (right)");
        ioWriter = new IOScript();
        headPositions = new List<Vector3>();
        controllerLeftPositions = new List<Vector3>();
        controllerRightPositions = new List<Vector3>();

        headRotations = new List<Vector3>();
        controllerLeftRotations = new List<Vector3>();
        controllerRightRotations = new List<Vector3>();

        times = new List<float>();

        timer = 0f;
        recording = false;
        recordingIndex = 0;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (recording)
        {
            headPositions.Add(head.position);
            headRotations.Add(head.eulerAngles);
            controllerLeftPositions.Add(controllerLeft.position);
            controllerLeftRotations.Add(controllerLeft.eulerAngles);
            controllerRightPositions.Add(controllerRight.position);
            controllerRightRotations.Add(controllerRight.eulerAngles);
            timer += Time.deltaTime;
            times.Add(timer);
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            if(recording)
            {
                recording = false;
                recordingIndex++;

                ioWriter.WriteToFileAsCSV(startTime, "head", recordingIndex, headPositions, headRotations, times);
                ioWriter.WriteToFileAsCSV(startTime, "left", recordingIndex, controllerLeftPositions, controllerLeftRotations, times);
                ioWriter.WriteToFileAsCSV(startTime, "right", recordingIndex, controllerRightPositions, controllerRightRotations, times);

                headPositions.Clear();
                headRotations.Clear();
                controllerLeftPositions.Clear();
                controllerLeftRotations.Clear();
                controllerRightPositions.Clear();
                controllerRightRotations.Clear();
                times.Clear();
                timer = 0f;
                Debug.Log("Written");

            }
            else if(!recording)
            {
                startTime = System.DateTime.Now;
                recording = true;
            }
        }


    }
}
