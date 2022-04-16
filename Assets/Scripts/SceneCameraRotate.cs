using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCameraRotate : MonoBehaviour {
    int pos = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        pos += 1;
        transform.position += transform.right*0.15f;
        transform.localEulerAngles += new Vector3(0f, -0.15f, 0f);
    }
}
