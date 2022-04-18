using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public int id;
    public string title = "Monster";
    public int AC = 9;
    public int HP = 10;
    public int WALK_SPEED = 30;
    public int CLIMB_SPEED = 0;
    public int FLY_SPEED = 0;
    public bool HOVER = false;
    public int CR = 0;
    public float attackSpeed = 1f;
    public int fov = 60;
    public Vector3 rotate;
    public Vector3 offset;
    public GameObject[] primaryEmitters;
    public GameObject[] secondaryEmitters;
    public GameObject[] tertiaryEmitters;
    public GameObject[] hideFirstPerson;
    public GameObject[] hideThirdPerson;
    public GameObject camAttach;
    public bool rotateWithCamera = false;
    public float distanceToGround = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
