using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipFloor : MonoBehaviour {

    public int floorNum = 0;

    public GameObject stairs;
    public float maxPlayerDist = 4.5f;

    public bool stepsOpen = true;
    public int stepsDir = 0;

    public Vector3[] stepOgPos = new Vector3[30];
    public GameObject[] stepObj = new GameObject[30];

    public GameObject above;
    public GameObject below;

    public GameObject hitbox;
    public GameObject lever;
    public GameObject leverMesh;

    public GameObject stairApprox;

    public static Dictionary<GameObject, int> leverMap = new Dictionary<GameObject, int>();
    public static Dictionary<int, GameObject> floorMap = new Dictionary<int, GameObject>();

    public void toggleSteps()
    {
        if (stepsOpen)
        {
            stepsDir = 1;
            
        }
        else
        {
            stepsDir = -1;
        }
    }

    // Use this for initialization
    void Start () {
        GameObject e = stairs;
		for (int i = 0; i < stepObj.Length; i++)
        {
            e.transform.position += Vector3.up * 0.003f;
            stepOgPos[i] = e.transform.position;
            stepObj[i] = e;
            e.AddComponent<BoxCollider>();
            if (i > 0)
                e.GetComponent<BoxCollider>().isTrigger = true;
            e.tag = "Stairs2";
            Debug.Log(i);
            if (i < stepObj.Length-1)
                e = e.transform.GetChild(0).gameObject;
        }
        lever.tag = "Lever";
        leverMap.Add(lever, floorNum);
        floorMap.Add(floorNum, this.gameObject);
        stairApprox = GameObject.Instantiate(stairApprox);
        stairApprox.transform.SetParent(stepObj[0].transform);
        stairApprox.transform.localPosition = new Vector3(0.008f, -0.0368f, -0.0805f);
        stairApprox.transform.localEulerAngles = new Vector3(90, 0, -30.054f);
        stairApprox.transform.localScale = new Vector3(0.08412058f, 0.001728325f, 0.01168141f);
        //stepsDir = 1;
    }

    private void CheckRender()
    {
        if (Player.localPlayer != null)
        {
            
                //Debug.Log("Checking render, player pos.y = " + Player.localPlayer.transform.position.y);
                if (Player.localPlayer.transform.position.y > transform.position.y - maxPlayerDist * 0.8f && below != null)
                    below.SetActive(false);
                else if (below != null)
                    below.SetActive(true);
                if (Player.localPlayer.transform.position.y < transform.position.y - maxPlayerDist * 1.8f && above != null)
                    above.SetActive(false);
                else if (above != null)
                    above.SetActive(true);
            //below.gameObject. = false;
        }
    }

    private void OnDisable()
    {
        hitbox.SetActive(false);
        CheckRender();
    }

    private void OnEnable()
    {
        hitbox.SetActive(true);
    }

    //int fuc = 0;

    void FixedUpdate () {
        CheckRender();
        if (stepObj.Length > 0 && stepsDir != 0) {
            bool yeet = false;
            if (stepsDir > 0 && leverMesh.transform.localEulerAngles.z > 10)
            {
                leverMesh.transform.localEulerAngles += -5 * Vector3.forward;
            } else if (stepsDir < 0 && leverMesh.transform.localEulerAngles.z < 105)
            {
                leverMesh.transform.localEulerAngles += 5 * Vector3.forward;
            }
            for (int i = 1; i < stepObj.Length; i++)
            {
                if ((stepsDir > 0 && stepObj[stepObj.Length-i].transform.position.y < stepObj[stepObj.Length-1-i].transform.position.y) || (stepsDir < 0 && stepObj[i].transform.position.y > stepOgPos[i].y))
                {
                    stepObj[stepsDir < 0 ? i : stepObj.Length - i].GetComponent<BoxCollider>().isTrigger = stepsDir < 0;
                    stepObj[stepsDir < 0 ? i : stepObj.Length - i].transform.position += stepsDir * Vector3.up * 0.0073f;
                    yeet = true;
                    break;
                }
            }
            stepsOpen = stepsDir < 0;
            if (!yeet)
            {
                //Debug.Log("StepsDir = " + stepsDir);
                if (stairApprox != null)
                    stairApprox.SetActive(stepsDir < 0);
                stepsDir = 0;
            }
        }
        //fuc++;
    }
}
