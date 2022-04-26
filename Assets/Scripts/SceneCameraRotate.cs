using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneCameraRotate : MonoBehaviour {
    int pos = 0;
    // Use this for initialization

    public GameObject[] playables;
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

    float[][] scalers = new float[][] {
        new float[] { 1.8f },
        new float[] { 0.8f, 0.8f,  0.8f},
        new float[] { 1.7f, 1.7f },
        new float[] { 1.3f }
    };

    float[][] translaters = new float[][]
    {
        new float[] { 0 },
        new float[] { 0, 0, 0 },
        new float[] { -0.7f, -0.7f },
        new float[] { -1.3f }
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
        tempModel = Instantiate(playables[charId], charDisplay.transform.position, charDisplay.transform.rotation);
        tempModel.transform.SetParent(charDisplay.transform);
        if (tempModel.GetComponent<Character>().camAttach != null)
        {
            tempGun = Instantiate(tempModel.GetComponent<Character>().camAttach, tempModel.transform.position, tempModel.transform.rotation);
            tempGun.transform.SetParent(tempModel.transform);
        }
        charText.GetComponent<Text>().text = tempModel.GetComponent<Character>().title + "\n\nHP: " + tempModel.GetComponent<Character>().HP + "\nAC: " + tempModel.GetComponent<Character>().AC + "\nWalk Speed: " + tempModel.GetComponent<Character>().WALK_SPEED + "\nClimb Speed: " + tempModel.GetComponent<Character>().CLIMB_SPEED + "\nFly Speed: " + tempModel.GetComponent<Character>().FLY_SPEED + "\nHover: " + tempModel.GetComponent<Character>().HOVER;
        if (GameManager.instance != null)
            GameManager.instance.charId = charId;
        if (ii < scalers.Length && jj < scalers[ii].Length)
            tempModel.transform.localScale *= scalers[ii][jj];
        if (ii < translaters.Length && jj < translaters[ii].Length)
            tempModel.transform.position += Vector3.up * translaters[ii][jj];
    }

    // Update is called once per frame
    void Update () {
        if (tempModel.GetComponent<Character>().FLY_SPEED > 0)
            tempModel.GetComponent<Character>().creature.flyAnim(0.5f);
        else
            tempModel.GetComponent<Character>().creature.walkAnim(0.5f);
        pos += 1;
        transform.position += transform.right*0.05f;
        transform.localEulerAngles += new Vector3(0f, -0.05f, 0f);
        //if (tempModel == null && GameManager.instance != null && playables[0] != null)
        //{
        //    ii = 0;
        //    jj = 0;
        //    Begin();
        //}
        if (tempModel != null)
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
