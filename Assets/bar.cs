using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bar : MonoBehaviour {

    public GameObject slider;

    public float pos = 1;

    public float x, w=-1;

    RectTransform rt;

    public void setPos(float newPos)
    {
        pos = newPos;
        updatePos();
    }

    public float getPos()
    {
        return pos;
    }

    public void updatePos()
    {
        if (w != -1)
        {
            Debug.Log("Pos = " + pos);
            slider.transform.position = new Vector3(x - pos * w / 2, slider.transform.position.y, slider.transform.position.z);
            slider.transform.localScale = new Vector3(pos * w, slider.transform.localScale.y, slider.transform.localScale.z);
            //rt.transform.position = new Vector3(x + pos * w / 2, rt.transform.position.y, rt.transform.position.z);
            //rt.transform.localScale = new Vector3(pos * w, rt.transform.localScale.y, rt.transform.localScale.z);

        }
    }

	// Use this for initialization
	void Start ()
    {
        //rt = slider.GetComponent<RectTransform>();
        w = slider.transform.localScale.x;
        x = slider.transform.position.x - w / 2;
    }

    private void Update()
    {
        updatePos();
    }

}
