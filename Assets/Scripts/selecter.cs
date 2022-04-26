using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selecter : MonoBehaviour {

    public float value = 0.0f;
    public Button plus;
    public Button minus;
    public GameObject valueTxt;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        valueTxt.GetComponent<Text>().text = "" + value;
        
	}

    public void plusClick()
    {
        value += 0.1f;
    }

    public void minusClick()
    {
        value = Mathf.Max(0, value - 0.1f);
    }
}
