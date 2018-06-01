using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //this.gameObject.transform.Rotate(GameObject.Find("Cube (1)").transform.position, -GameObject.Find("Cube (1)").transform.eulerAngles.y)
        this.gameObject.transform.RotateAround(GameObject.Find("Cube (1)").transform.position, Vector3.up, -GameObject.Find("Cube (1)").transform.eulerAngles.y);
        GameObject.Find("Cube (1)").transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
}
