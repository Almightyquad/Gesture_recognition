using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReadScript : MonoBehaviour {
    bool done = false;

    FileInfo sourceFile;
    StreamReader fileReader;

    List<Vector3> vectorList;

    List<Vector3> posVectorList;
    List<Vector3> rotVectorList;
    List<float> timesList;

    string timestamp;
    int counter = 0;
    GameObject controllerDummy;
    // Use this for initialization
    void Start () {

        vectorList = new List<Vector3>();
        posVectorList = new List<Vector3>();
        rotVectorList = new List<Vector3>();

        timesList = new List<float>();
        sourceFile = new FileInfo("Assets/GestureData/Recording1/recordingright5.txt");
        fileReader = sourceFile.OpenText();
        readVectors(fileReader, ref vectorList, ref timestamp);
        controllerDummy = GameObject.Find("Controller (dummy)");
    }
	
	// Update is called once per frame
	void Update () {
        if (counter < posVectorList.Count)
        {
            controllerDummy.transform.position = posVectorList[counter];
            controllerDummy.transform.eulerAngles = rotVectorList[counter];
            counter++;

        }
	}

	
    public void readFile(string fileName, ref List<Vector3> posVectorList, ref List<Vector3> rotVectorList, ref List<float> timesList, ref string timestamp)
    {
        sourceFile = new FileInfo(fileName);
        fileReader = sourceFile.OpenText();
        readVectors(fileReader, ref posVectorList, ref rotVectorList, ref timesList, ref timestamp);
        fileReader.Close();
    }
	
	
    void readVectors(StreamReader reader, ref List<Vector3> posVectorList, ref List<Vector3> rotVectorList, ref List<float> timesList, ref string timestamp)
    {
        string text = "";
        posVectorList = new List<Vector3>();
        rotVectorList = new List<Vector3>();
        timesList = new List<float>();
        string[] stringList;

        timestamp = reader.ReadLine();

        //Not very proud of this while setup -.-
        //TODO: I don't think it's needed, just check later
        while (!done)
        {
            text = reader.ReadToEnd();
            stringList = text.Split('\n');
            int lengthOfArrays = (stringList.Length - 1) / 3;
            string[] tempStrList;
            for (int i = 0; i < stringList.Length - 1; i++)
            {
                if(i < lengthOfArrays)
                {
                    tempStrList = stringList[i].Split(',');
                    posVectorList.Add(new Vector3(float.Parse(tempStrList[0]), float.Parse(tempStrList[1]), float.Parse(tempStrList[2])));
                }
                else if (i >= lengthOfArrays && i < lengthOfArrays * 2)
                {
                    tempStrList = stringList[i].Split(',');
                    rotVectorList.Add(new Vector3(float.Parse(tempStrList[0]), float.Parse(tempStrList[1]), float.Parse(tempStrList[2])));
                }
                else
                {
                    timesList.Add(float.Parse(stringList[i]));
                }
            }
            /*if ((text = reader.ReadLine()) != null)
            {
                //text.Split is so useful! Love that method.
                stringList = text.Split(',');
                vectorList.Add(new Vector3(float.Parse(stringList[0]), float.Parse(stringList[1]), float.Parse(stringList[2])));
            }
            else
            {
                done = true;
                Debug.Log(vectorList.Count);
            }*/
            done = true;
        }
        
        done = false;
    }
	
	
    void readVectors(StreamReader reader, ref List<Vector3> vectorList, ref string timestamp)
    {
        string text = " ";
        vectorList = new List<Vector3>();
        string[] stringList;

        timestamp = reader.ReadLine();

        //Not very proud of this while setup -.-
        while (!done)
        {
            text = reader.ReadToEnd();
            stringList = text.Split('\n');
            int lengthOfArrays = (stringList.Length - 1) / 3;
            string[] tempStrList;
            for (int i = 0; i < stringList.Length - 1; i++)
            {
                if(i < lengthOfArrays)
                {
                    tempStrList = stringList[i].Split(',');
                    posVectorList.Add(new Vector3(float.Parse(tempStrList[0]), float.Parse(tempStrList[1]), float.Parse(tempStrList[2])));
                }
                else if (i >= lengthOfArrays && i < lengthOfArrays * 2)
                {
                    tempStrList = stringList[i].Split(',');
                    rotVectorList.Add(new Vector3(float.Parse(tempStrList[0]), float.Parse(tempStrList[1]), float.Parse(tempStrList[2])));
                }
                else
                {
                    timesList.Add(float.Parse(stringList[i]));
                }
            }
            /*if ((text = reader.ReadLine()) != null)
            {
                //text.Split is so useful! Love that method.
                stringList = text.Split(',');
                vectorList.Add(new Vector3(float.Parse(stringList[0]), float.Parse(stringList[1]), float.Parse(stringList[2])));
            }
            else
            {
                done = true;
                Debug.Log(vectorList.Count);
            }*/
            done = true;
        }
        
        done = false;
    }

}
