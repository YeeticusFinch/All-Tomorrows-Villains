using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {

    public int floorNum = 0;

    public Material green;
    public Material red;

    public bool on = true;

    public static Dictionary<int, GameObject> floorElevatorMap = new Dictionary<int, GameObject>();
    //public static Dictionary<GameObject, int> elevatorFloorMap = new Dictionary<GameObject, int>();

    // Use this for initialization
    void Start () {
        floorElevatorMap.Add(floorNum, this.gameObject);
        this.gameObject.AddComponent<BoxCollider>();
        this.gameObject.tag = "Elevator";
        GetComponent<MeshRenderer>().materials = CarlMath.arrayCombine(GetComponent<MeshRenderer>().materials, new Material[] { green });
    }
	
	public void TOggleOn()
    {
        on = !on;
        if (on)
        {
            GetComponent<MeshRenderer>().materials[2].color = Color.green;
            GetComponent<MeshRenderer>().materials[2].SetColor("_EmissionColor", Color.green*4);
        }
        else
        {
            GetComponent<MeshRenderer>().materials[2].color = Color.red;
            GetComponent<MeshRenderer>().materials[2].SetColor("_EmissionColor", Color.red*4);
        }
    }
}
