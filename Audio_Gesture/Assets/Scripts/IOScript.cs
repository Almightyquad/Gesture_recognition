using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IOScript {

    bool done;

    void Awake()
    {
        done = false;
    }
    //I pretty much ripped the reading from some stackoverflow post and customized it. It is really ugly and should be fixed.
    void readFiles(StreamReader reader, ref List<Quaternion> quaternionList, ref List<long> times)
    {
        string text = " ";
        quaternionList = new List<Quaternion>();
        string[] stringList;
        //Not very proud of this while setup -.-
        while (!done)
        {
            if ((text = reader.ReadLine()) != null)
            {
                //text.Split is so useful! Love that method.
                stringList = text.Split(',', ' ');
                quaternionList.Add(new Quaternion(float.Parse(stringList[0]), float.Parse(stringList[1]), float.Parse(stringList[2]), float.Parse(stringList[3])));
                times.Add(Int64.Parse(stringList[5]));
            }
            else
            {
                done = true;
                Debug.Log(quaternionList.Count);
            }
        }
        done = false;
    }

    //Converting to CSV is incredibly useful when working with stupid programs that require excel documents.
    //The lists of positions, rotations and times should be equal in length
    public void WriteToFileAsCSV(System.DateTime startTime, string fileName, int recordingIndex, List<Vector3> positions, List<Vector3> rotations, List<float> times)
    {
        //Find the applications datapath and append the folder moCapData to it.
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GestureData/RecordingsTemp");
        if(!dir.Exists)
        {
            dir.Create();
        }
        //string[] tempPositionList = new string[positions.Count];
        //string[] tempRotationsList = new string[rotations.Count];
        //string[] tempTimesList = new string[times.Count];
        string tempString = "" + startTime + "\n";

        //I don't know why I needed the second CSV format. I made that thing first for some reason. It is there if I decide I need it.
        for (int i = 0; i < positions.Count; i++)
        {
            //First CSV
            //x,y,z,w
            //x,y,z,w
            //x,y,z,w
            tempString = tempString + positions[i].x + "," + positions[i].y + "," + positions[i].z + "\n";
        }
        for (int i = 0; i < positions.Count; i++)
        {
            tempString = tempString + rotations[i].x + "," + rotations[i].y + "," + rotations[i].z + "\n";
        }
        for (int i = 0; i < positions.Count; i++)
        {
            tempString = tempString + times[i] + "\n";
        }

        //Just toss the data into the root folder. No clutter.
        File.WriteAllText(Application.dataPath + "/GestureData/RecordingsTemp/Recording" + fileName + recordingIndex + ".txt", tempString);
    }

    public void WriteToFileAsCSV(string fileName, int recordingIndex, List<float> scores)
    {
        //Find the applications datapath and append the folder moCapData to it.
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GestureData/RecordingsTemp");
        if (!dir.Exists)
        {
            dir.Create();
        }
        //string[] tempPositionList = new string[positions.Count];
        //string[] tempRotationsList = new string[rotations.Count];
        //string[] tempTimesList = new string[times.Count];
        string tempString = "";

        //I don't know why I needed the second CSV format. I made that thing first for some reason. It is there if I decide I need it.
        for (int i = 0; i < scores.Count; i++)
        {
            List<float> lineScore = new List<float>();
            //First CSV
            //x,y,z,w
            //x,y,z,w
            //x,y,z,w

            /*for(int j = 0; j < scores.Count; j++)
            {
                lineScore.Add(scores[j][i]);
            }
            tempString = tempString + (i + 1) + " ";
            for (int j = 0; j < lineScore.Count; j++)
            {
                tempString = tempString + lineScore[j] + " ";
            }
            tempString = tempString + "\n";*/

            //Lets just conform to gnuplot standards -.-
            tempString = tempString + (i + 1) + " " + scores[i] + "\n";
        }
        //Just toss the data into the root folder. No clutter.
        File.WriteAllText(Application.dataPath + "/GestureData/RecordingsTemp/Recording" + fileName + recordingIndex + ".txt", tempString);
    }

    public void WriteToFileAsCSV(string fileName, int recordingIndex, List<List<float>> scores, List<string> templateNames)
    {
        //Find the applications datapath and append the folder moCapData to it.
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GestureData/RecordingsTemp");
        if (!dir.Exists)
        {
            dir.Create();
        }
        //string[] tempPositionList = new string[positions.Count];
        //string[] tempRotationsList = new string[rotations.Count];
        //string[] tempTimesList = new string[times.Count];
        string tempString = "";

        //I don't know why I needed the second CSV format. I made that thing first for some reason. It is there if I decide I need it.
        for (int j = 0; j < scores[0].Count; j++)
        {
            tempString = tempString + "\"" + templateNames[j] + (j+1) + "\"" + "\n";
            for (int i = 0; i < scores.Count; i++)
            {
                //First CSV
                //x,y,z,w
                //x,y,z,w
                //x,y,z,w

                /*for(int j = 0; j < scores.Count; j++)
                {
                    lineScore.Add(scores[j][i]);
                }
                tempString = tempString + (i + 1) + " ";
                for (int j = 0; j < lineScore.Count; j++)
                {
                    tempString = tempString + lineScore[j] + " ";
                }
                tempString = tempString + "\n";*/

                //Lets just conform to gnuplot standards -.-
                tempString = tempString + (i + 1) + " " + scores[i][j] + "\n";
            }
            tempString = tempString + "\n\n";
        }
        //Just toss the data into the root folder. No clutter.
        File.WriteAllText(Application.dataPath + "/GestureData/RecordingsTemp/Recording" + fileName + recordingIndex + ".txt", tempString);
    }


}
