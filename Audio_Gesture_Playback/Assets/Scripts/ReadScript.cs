using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadScript : MonoBehaviour {

    IOScript ios;

    FileInfo sourceFile;
    StreamReader fileReader;

    List<PlaybackEvent> playbackEvents;

    //Which recording
    int recordingNumber;

    //Which recordingEventNumber
    int recordingEventNumber;

    class PlaybackEvent
    {
        //TODO: Might be better to create an instance of the object with specifications
        GameObject playbackGameObject;
        public PlaybackVariables vars;
        public int counter;
        string timestamp;
        //Needs the game objects name
        public PlaybackEvent(string objectName, StreamReader reader, string assetName, IOScript ios)
        {
            playbackGameObject = GameObject.Find(objectName);
            vars = new PlaybackVariables();
            vars.posVectorList = new List<Vector3>();
            vars.rotVectorList = new List<Vector3>();
            vars.timesList = new List<float>();
            counter = 0;
            readData(reader, assetName, ios);
        }

        public void updateObject()
        {
            playbackGameObject.transform.position = vars.posVectorList[counter];
            playbackGameObject.transform.eulerAngles = vars.rotVectorList[counter];
            //TODO: Counter should probably be shared
            counter++;
        }
        //TODO: Currently Empty
        public void resetObject()
        { }

        void readData(StreamReader reader, string assetName, IOScript ios)
        {
            FileInfo sourceFile;
            sourceFile = new FileInfo(assetName);
            reader = sourceFile.OpenText();
            ios.readVectors(reader, ref vars.posVectorList, ref vars.rotVectorList, ref vars.timesList, ref timestamp);
            reader.Close();
        }

        public struct PlaybackVariables
        {
            public List<Vector3> posVectorList;
            public List<Vector3> rotVectorList;
            public List<float> timesList;
        }
    }

    // Use this for initialization
    void Start () {
        recordingNumber = 1;
        recordingEventNumber = 1;
        ios = new IOScript();
        newPlayback();
    }
	
    void newPlayback()
    {
        playbackEvents = new List<PlaybackEvent>();
        playbackEvents.Add(new PlaybackEvent("Controller (dummyright)", fileReader, "Assets/GestureData/Recording" + recordingNumber + "/recordingright" + recordingEventNumber + ".txt", ios));
        playbackEvents.Add(new PlaybackEvent("Controller (dummyleft)", fileReader, "Assets/GestureData/Recording" + recordingNumber + "/recordingleft" + recordingEventNumber + ".txt", ios));
        playbackEvents.Add(new PlaybackEvent("Controller (head)", fileReader, "Assets/GestureData/Recording" + recordingNumber + "/recordinghead" + recordingEventNumber + ".txt", ios));
    }

	// Update is called once per frame
	void Update () {
        if (playbackEvents[0].counter < playbackEvents[0].vars.posVectorList.Count)
        {
            playbackEvents[0].updateObject();
            playbackEvents[1].updateObject();
            playbackEvents[2].updateObject();
        }

        if(Input.GetKeyUp(KeyCode.LeftArrow))
        {
            recordingEventNumber--;
            //Hacky
            if (recordingEventNumber == 0)
            {
                recordingEventNumber++;
            }
            newPlayback();
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            recordingEventNumber++;
            //Hacky
            if (recordingEventNumber == 14)
            {
                recordingEventNumber--;
            }
            newPlayback();
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            recordingNumber--;
            //Hacky
            if (recordingNumber == 0)
            {
                recordingNumber++;
            }
            newPlayback();
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            recordingNumber++;
            //Hacky
            if (recordingNumber == 23)
            {
                recordingNumber--;
            }
            newPlayback();
        }

    }



    

}
