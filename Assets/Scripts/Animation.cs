using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour {

    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1"))
        {
            anim.Play("Bite");
        }
        else if (Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical") != 0)
        {
            anim.Play("Walking");
        }
    }
}
