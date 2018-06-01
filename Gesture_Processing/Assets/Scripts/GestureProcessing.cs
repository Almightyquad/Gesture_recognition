using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GestureProcessing : MonoBehaviour {

    class GestureData
    {
        public int truePositive;
        public int falsePositive;
        public int trueNegative;
        public int falseNegative;

        public GestureData()
        {
            truePositive = 0;
            falsePositive = 0;
            trueNegative = 0;
            falseNegative = 0;
        }
    }

    List<List<List<float>>> gestureOne;
    List<List<List<float>>> gestureTwo;
    List<List<List<float>>> gestureThree;
    List<List<List<float>>> gestureFour;
    List<List<List<float>>> gestureFive;

    List<GestureData> gestureData;

    public float threshold;
    public string saveName;

    // Use this for initialization
    void Start ()
    {
        gestureOne = new List<List<List<float>>>();
        gestureTwo = new List<List<List<float>>>();
        gestureThree = new List<List<List<float>>>();
        gestureFour = new List<List<List<float>>>();
        gestureFive = new List<List<List<float>>>();
        gestureData = new List<GestureData>();
        for (int i = 0; i < 5; i++)
        {
            gestureData.Add(new GestureData());
        }
        readData();
    }
	
	// Update is called once per frame
	void Update () {
		
    }

    void readData()
    {
        string truePath = Application.dataPath + "/Data";
        string path = Application.dataPath + "/Data";
        path += "/Test1/Recordingtemplate1.txt";
        StreamReader reader;
        for(int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                Debug.Log(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                reader = new StreamReader(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                gestureOne.Add(readVectors(reader));
                reader.Close();
            }
        }
        for (int i = 1; i < 11; i++)
        {
            for (int j = 4; j < 7; j++)
            {
                Debug.Log(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                reader = new StreamReader(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                gestureTwo.Add(readVectors(reader));
                reader.Close();
            }
        }
        for (int i = 1; i < 11; i++)
        {
            for (int j = 7; j < 10; j++)
            {
                Debug.Log(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                reader = new StreamReader(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                gestureThree.Add(readVectors(reader));
                reader.Close();
            }
        }
        for (int i = 1; i < 11; i++)
        {
            for (int j = 10; j < 13; j++)
            {
                Debug.Log(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                reader = new StreamReader(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                gestureFour.Add(readVectors(reader));
                reader.Close();
            }
        }
        for (int i = 1; i < 11; i++)
        {
            for (int j = 13; j < 16; j++)
            {
                Debug.Log(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                reader = new StreamReader(truePath + "/Test" + i + "/Recordingtemplate" + j + ".txt");
                gestureFive.Add(readVectors(reader));
                reader.Close();
            }
        }
        List<float> thresholdList = new List<float>();
        for(float i = 0f; i < 1; i += 0.1f)
        {
            thresholdList.Add(i);
        }
        thresholdList.Add(1f);
        thresholdList.Add(1.5f);
        for(int i = 2; i < 20; i++)
        {
            thresholdList.Add(i);
        }
        funWithThresholds(thresholdList);

    }

    void funWithThresholds(float threshold)
    {
        //speak
        for (int i = 0; i < gestureOne.Count; i++)
        {
            //headBringToLeft
            falseGesture(threshold, gestureOne[i][0], 0);

            //headCircle
            falseGesture(threshold, gestureOne[i][1], 1);

            //headDrag
            falseGesture(threshold, gestureOne[i][2], 2);

            //headSpeak
            trueGesture(threshold, gestureOne[i][3], 3);

            //headSwipe
            falseGesture(threshold, gestureOne[i][4], 4);

        }
        writeToFile(gestureData, "Speak" + threshold, 3, threshold);
        resetGestureData();
        //Swipe
        for (int i = 0; i < gestureTwo.Count; i++)
        {
            //headBringToLeft
            falseGesture(threshold, gestureTwo[i][0], 0);

            //headCircle
            falseGesture(threshold, gestureTwo[i][1], 1);

            //headDrag
            falseGesture(threshold, gestureTwo[i][2], 2);

            //headSpeak
            falseGesture(threshold, gestureTwo[i][3], 3);

            //headSwipe
            trueGesture(threshold, gestureTwo[i][4], 4);

        }
        writeToFile(gestureData, "Swipe" + threshold, 4, threshold);
        resetGestureData();

        //drag
        for (int i = 0; i < gestureThree.Count; i++)
        {
            //headBringToLeft
            falseGesture(threshold, gestureThree[i][0], 0);

            //headCircle
            falseGesture(threshold, gestureThree[i][1], 1);

            //headDrag
            trueGesture(threshold, gestureThree[i][2], 2);

            //headSpeak
            falseGesture(threshold, gestureThree[i][3], 3);

            //headSwipe
            falseGesture(threshold, gestureThree[i][4], 4);

        }
        writeToFile(gestureData, "Drag" + threshold, 2, threshold);
        resetGestureData();

        //sheath
        for (int i = 0; i < gestureFour.Count; i++)
        {
            //headBringToLeft
            trueGesture(threshold, gestureFour[i][0], 0);

            //headCircle
            falseGesture(threshold, gestureFour[i][1], 1);

            //headDrag
            falseGesture(threshold, gestureFour[i][2], 2);

            //headSpeak
            falseGesture(threshold, gestureFour[i][3], 3);

            //headSwipe
            falseGesture(threshold, gestureFour[i][4], 4);

        }
        writeToFile(gestureData, "Sheath" + threshold, 0, threshold);
        resetGestureData();

        //circle
        for (int i = 0; i < gestureFive.Count; i++)
        {
            //headBringToLeft
            falseGesture(threshold, gestureFive[i][0], 0);

            //headCircle
            trueGesture(threshold, gestureFive[i][1], 1);

            //headDrag
            falseGesture(threshold, gestureFive[i][2], 2);

            //headSpeak
            falseGesture(threshold, gestureFive[i][3], 3);

            //headSwipe
            falseGesture(threshold, gestureFive[i][4], 4);

        }
        writeToFile(gestureData, "Circle" + threshold, 1, threshold);
        resetGestureData();

    }



    void funWithThresholds(List<float> threshold)
    {
        List<List<GestureData>> gestureComp = new List<List<GestureData>>();
        //speak
        for (int j = 0; j < threshold.Count; j++)
        {
            for (int i = 0; i < gestureOne.Count; i++)
            {
                //headBringToLeft
                falseGesture(threshold[j], gestureOne[i][0], 0);

                //headCircle
                falseGesture(threshold[j], gestureOne[i][1], 1);

                //headDrag
                falseGesture(threshold[j], gestureOne[i][2], 2);

                //headSpeak
                trueGesture(threshold[j], gestureOne[i][3], 3);

                //headSwipe
                falseGesture(threshold[j], gestureOne[i][4], 4);

            }
            gestureComp.Add(gestureData);
            resetGestureData();
        }
        writeToFile(gestureComp, "Speak", 3, threshold);
        resetGestureData();
        gestureComp.Clear();
        //Swipe
        for (int j = 0; j < threshold.Count; j++)
        {
            for (int i = 0; i < gestureTwo.Count; i++)
            {
                //headBringToLeft
                falseGesture(threshold[j], gestureTwo[i][0], 0);

                //headCircle
                falseGesture(threshold[j], gestureTwo[i][1], 1);

                //headDrag
                falseGesture(threshold[j], gestureTwo[i][2], 2);

                //headSpeak
                falseGesture(threshold[j], gestureTwo[i][3], 3);

                //headSwipe
                trueGesture(threshold[j], gestureTwo[i][4], 4);

            }
            gestureComp.Add(gestureData);
            Debug.Log(gestureData[0]);
            resetGestureData();
        }
        writeToFile(gestureComp, "Swipe", 4, threshold);
        resetGestureData();
        gestureComp.Clear();

        //drag
        for (int j = 0; j < threshold.Count; j++)
        {
            for (int i = 0; i < gestureThree.Count; i++)
            {
                //headBringToLeft
                falseGesture(threshold[j], gestureThree[i][0], 0);

                //headCircle
                falseGesture(threshold[j], gestureThree[i][1], 1);

                //headDrag
                trueGesture(threshold[j], gestureThree[i][2], 2);

                //headSpeak
                falseGesture(threshold[j], gestureThree[i][3], 3);

                //headSwipe
                falseGesture(threshold[j], gestureThree[i][4], 4);

            }
            gestureComp.Add(gestureData);
            resetGestureData();
        }
        writeToFile(gestureComp, "Drag", 2, threshold);
        resetGestureData();
        gestureComp.Clear();

        //sheath
        for (int j = 0; j < threshold.Count; j++)
        {
            for (int i = 0; i < gestureFour.Count; i++)
            {
                //headBringToLeft
                trueGesture(threshold[j], gestureFour[i][0], 0);

                //headCircle
                falseGesture(threshold[j], gestureFour[i][1], 1);

                //headDrag
                falseGesture(threshold[j], gestureFour[i][2], 2);

                //headSpeak
                falseGesture(threshold[j], gestureFour[i][3], 3);

                //headSwipe
                falseGesture(threshold[j], gestureFour[i][4], 4);

            }
            gestureComp.Add(gestureData);
            resetGestureData();
        }
        writeToFile(gestureComp, "Sheath", 0, threshold);
        resetGestureData();
        gestureComp.Clear();

        //circle
        for (int j = 0; j < threshold.Count; j++)
        {
            for (int i = 0; i < gestureFive.Count; i++)
            {
                //headBringToLeft
                falseGesture(threshold[j], gestureFive[i][0], 0);

                //headCircle
                trueGesture(threshold[j], gestureFive[i][1], 1);

                //headDrag
                falseGesture(threshold[j], gestureFive[i][2], 2);

                //headSpeak
                falseGesture(threshold[j], gestureFive[i][3], 3);

                //headSwipe
                falseGesture(threshold[j], gestureFive[i][4], 4);

            }
            gestureComp.Add(gestureData);
            resetGestureData();
        }
        writeToFile(gestureComp, "Circle", 1, threshold);
        resetGestureData();
        gestureComp.Clear();

    }



    void trueGesture(float threshold, List<float> gestureFloatData, int gestureIdent)
    {
        if (findThreshold(threshold, gestureFloatData))
        {
            gestureData[gestureIdent].truePositive += 1;
        }
        else
        {
            gestureData[gestureIdent].falseNegative += 1;
        }
    }

    void falseGesture(float threshold, List<float> gestureFloatData, int gestureIdent)
    {
        if (findThreshold(threshold, gestureFloatData))
        {
            gestureData[gestureIdent].falsePositive += 1;
        }
        else
        {
            gestureData[gestureIdent].trueNegative += 1;
        }
    }

    bool findThreshold(float threshold, List<float> scoreList)
    {

        for (int i = 0; i < scoreList.Count; i++)
        {
            if(scoreList[i] < threshold)
            {
                return true;
            }
        }
        return false;
    }

    void writeToFile(List<GestureData> gestureData, string name, int gestureIdent, float threshold)
    {
        string stringBuilder = "GestureName,Sheath,Circle,Drag,Speak,Swipe,Threshold\n";
        stringBuilder += "Pos,";
        for(int i = 0; i < gestureData.Count; i++)
        {
            if(gestureIdent != i)
            {
                stringBuilder += gestureData[i].falsePositive;
            }
            else
            {
                stringBuilder += gestureData[i].truePositive;
            }
            stringBuilder += ",";
        }
        stringBuilder += threshold + "\n" + "Neg";
        for (int i = 0; i < gestureData.Count; i++)
        {
            stringBuilder += ",";
            if (gestureIdent != i)
            {
                stringBuilder += gestureData[i].trueNegative;
            }
            else
            {
                stringBuilder += gestureData[i].falseNegative;
            }
        }

        StreamWriter writer = new StreamWriter("Assets/csv/" + name + ".csv");
        writer.WriteLine(stringBuilder);
        writer.Close();

        
        //Format: FileName: Speak/Swipe/Drag/Sheath/Circle
        /*
        GestureName,    Sheath,    headCircle,     headDrag,   headSpeak,  headSwipe,   threshold
        Pos,                                                                            value
        Neg,            


        */
    }


    void writeToFile(List<List<GestureData>> gestureData, string name, int gestureIdent, List<float> threshold)
    {
        string stringBuilder = "";
        for (int j = 0; j < gestureData.Count; j++)
        {

            stringBuilder += "GestureName,Sheath,Circle,Drag,Speak,Swipe,Threshold\n";
            stringBuilder += "Pos,";
            for (int i = 0; i < gestureData[j].Count; i++)
            {
                if (gestureIdent != i)
                {
                    stringBuilder += gestureData[j][i].falsePositive;
                }
                else
                {
                    stringBuilder += gestureData[j][i].truePositive;
                }
                stringBuilder += ",";
            }
            stringBuilder += threshold[j] + "\n" + "Neg";
            for (int i = 0; i < gestureData[j].Count; i++)
            {
                stringBuilder += ",";
                if (gestureIdent != i)
                {
                    stringBuilder += gestureData[j][i].trueNegative;
                }
                else
                {
                    stringBuilder += gestureData[j][i].falseNegative;
                }
            }
            stringBuilder += "\n\n";

        }
        StreamWriter writer = new StreamWriter("Assets/csvComp/" + name + ".csv");
        writer.WriteLine(stringBuilder);
        writer.Close();


        //Format: FileName: Speak/Swipe/Drag/Sheath/Circle
        /*
        GestureName,    Sheath,    headCircle,     headDrag,   headSpeak,  headSwipe,   threshold
        Pos,                                                                            value
        Neg,            


        */
    }


    void resetGestureData()
    {
        gestureData = new List<GestureData>();
        for (int i = 0; i < 5; i++)
        {
            gestureData.Add(new GestureData());
        }
    }

    List<List<float>> readVectors(StreamReader reader)
    {
        string text = " ";
        List<List<float>> scoreList = new List<List<float>>();
        //string[] stringList;
        List<string> stringList = new List<string>();
        
        
        text = reader.ReadToEnd();
        stringList = new List<string>(text.Split('\n'));
        if(stringList[0][0] == '\"')
        {
            Debug.Log("Foshow");
        }

        int iterator = -1;
        Debug.Log(stringList[0] + " " + stringList[1]);
        for (int i = 0; i < stringList.Count - 1; i++)
        {
            if(stringList[i] == "")
            {
                Debug.Log("WOAH");
            }
            else if(stringList[i][0] == '\"')
            {
                Debug.Log(stringList[i]);
                iterator++;
                scoreList.Add(new List<float>());
            }
            else
            {
                scoreList[iterator].Add(float.Parse(stringList[i].Split(' ')[1]));
            }

        }
        return scoreList;
    }
}
