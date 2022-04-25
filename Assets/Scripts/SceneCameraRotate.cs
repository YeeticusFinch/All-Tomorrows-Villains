using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCameraRotate : MonoBehaviour {
    int pos = 0;
    // Use this for initialization

    
    public GameObject ui;
    public GameObject charText;
    public GameObject charDisplay;
    GameObject tempModel;
    GameObject tempGun;
    Canvas canvas;

    public int charId = 0;
    int[][] selArr = new int[][] { 
        new int[] { 0 }, 
        new int[] { 1, 2, 3 },
        new int[] { 4, 5 },
        new int[] { 6 }
    };

    int[][] scalers = new int[][] {
        new int[] { 4 },
        new int[] { 2, 2, 2 },
        new int[] { 4, 4 },
        new int[] { 5 }
    };

    int ii = 0;
    int jj = 0;

	void Start () {
        canvas = ui.GetComponent<Canvas>();
        updateChar();
    }
	
    int modClamp(int a, int b)
    {
        a %= b;
        while (a < 0)
            a += b;
        return a;
    }

    void updateChar()
    {
        ii = modClamp(ii, selArr.Length);
        jj = modClamp(jj, selArr[ii].Length);
        charId = selArr[ii][jj];
        GameObject.Destroy(tempModel);
        tempModel = Instantiate(GameManager.instance.playables[charId], charDisplay.transform.position, charDisplay.transform.rotation);
        tempModel.transform.SetParent(charDisplay.transform);
        if (tempModel.GetComponent<Character>().camAttach != null)
        {
            tempGun = Instantiate(tempModel.GetComponent<Character>().camAttach, tempModel.transform.position, tempModel.transform.rotation);
            tempGun.transform.SetParent(tempModel.transform);
        }
        GameManager.instance.charId = charId;
        //tempModel.transform.localScale *= scalers[ii][jj];
    }

    // Update is called once per frame
    void Update () {
        pos += 1;
        transform.position += transform.right*0.05f;
        transform.localEulerAngles += new Vector3(0f, -0.05f, 0f);
        tempModel.transform.localEulerAngles += new Vector3(0f, -0.5f, 0f);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ii--;
            jj = 0;
            updateChar();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ii++;
            jj = 0;
            updateChar();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jj++;
            updateChar();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            jj--;
            updateChar();
        }

    }
}
