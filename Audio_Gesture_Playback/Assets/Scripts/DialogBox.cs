using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour {
    GameObject objectTooltip;
    Canvas tooltipCanvas;
    /// <summary>
    /// The Dialog
    /// </summary>
    public string dialog;
    //Text width
    int textWidth = 22;
	// Use this for initialization
	void Start () {
        objectTooltip = Resources.Load<GameObject>("Prefabs/ObjectTooltip");
        
        objectTooltip = Instantiate(objectTooltip);

        //Find and set the correct position and orientation of the tooltip
        Vector3 tempVec3 = this.transform.position;
        tempVec3 = new Vector3(tempVec3.x, tempVec3.y + 1.5f, tempVec3.z);
        objectTooltip.transform.position = tempVec3;
        objectTooltip.transform.Rotate(0f, 90f, 0f);

        //Get the canvas under the Objecttooltip object for ease of use
        tooltipCanvas = objectTooltip.transform.GetComponentInChildren<Canvas>();

        //Set the lines position to be where the tooltip is
        objectTooltip.transform.FindChild("Line").transform.position = objectTooltip.transform.position;

        tempVec3 = new Vector3(this.transform.position.x, this.transform.position.y - 63f, this.transform.position.z - 2);
        GameObject tempButton = tooltipCanvas.transform.FindChild("Next Button").gameObject;
        tempButton.transform.position = tempVec3;
        tempVec3.z -= 1;
        tempButton.transform.FindChild("Text").transform.position = tempVec3;
        tempVec3.z += 2;
        tempButton.transform.FindChild("Text Back").transform.position = tempVec3;
        //Set size and scale of the tooltip
        tooltipCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(0.1f, 0.5f);
        tooltipCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);

        //Parse and set the text of the tooltip
        objectTooltip.GetComponent<VRTK.VRTK_ObjectTooltip>().displayText = parseDialog();
        //Set where the line from the tooltip is drawn to
        objectTooltip.GetComponent<VRTK.VRTK_ObjectTooltip>().drawLineTo = this.transform;
        //Set the button in the correct position
        var tmpText = tempButton.transform.FindChild("Text").GetComponent<Text>();
        tmpText.material = Resources.Load("UIText") as Material;
        tmpText = tempButton.transform.FindChild("Text Back").GetComponent<Text>();
        tmpText.material = Resources.Load("UIText") as Material;

        tempButton.GetComponent<Button>().onClick.AddListener(delegate { nextDialog(); });
    }
	
	// Update is called once per frame
	void Update () {

    }

    void nextDialog()
    {

    }

    string parseDialog()
    {
        string tempDialog = dialog;
        dialog = "";
        //Iterator to avoid infinite loops
        int iDontWantToRunForeverit = 0;
        int it = 0;
        //Fallback iterator in case there is a word that is the same size as the text width
        int fallbackit = 0;
        bool somethingsWrong = false;
        //As long as the tempdialog's length is higher than the textwidth, continue to parse
        while(tempDialog.Length > textWidth)
        {
            //Get the first textwidth of the string
            string tempStr = tempDialog.Substring(0, textWidth);
            //The check to see if there is a space at the start of it, if not, continue searching the string
            //until you find a space to correctly cut it into smaller pieces to fit within the tooltip
            while (!tempStr.EndsWith(" "))
            {
                if (it == textWidth + 1)
                {
                    somethingsWrong = true;
                }
                if (somethingsWrong)
                {
                    fallbackit--;
                    it = fallbackit;
                }
                else
                {
                    it++;
                }
                if( -it + textWidth == tempStr.Length)
                {
                    break;
                }
                tempStr = tempDialog.Substring(0, textWidth - it);
                iDontWantToRunForeverit++;
                if(iDontWantToRunForeverit == 10000)
                {
                    Debug.Log("Something went wrong");
                    break;
                }


            }
            dialog += tempStr + "\n";
            tempDialog = tempDialog.Remove(0, textWidth - it);
            it = 0;
            fallbackit = 0;
            iDontWantToRunForeverit++;
            if (iDontWantToRunForeverit == 10000)
            {
                Debug.Log("Something went wrong1");
                break;
            }
        }
        //Make sure that all the dialog is there.
        dialog += tempDialog;
        return dialog;
    }
}
