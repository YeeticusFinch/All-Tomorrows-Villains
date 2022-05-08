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
    Camera sceneCam;
    //public GameObject point;
    struct preset
    {
        public preset(string a, float[] b)
        {
            name = a;
            shit = b;
        }
        public string name;
        public float[] shit;
    }
    preset[] presets =
    {              // id  respawn  turn  speed  scale  damage  volume  pitch  attenuation
        new preset("Default", new float[]{0, 3, 2, 0.5f, 1, 1, 0.8f, 1, 1}),
        new preset("RAW D&D", new float[]{1, 6, 6, 1, 1, 1, 0.8f, 1, 1}),
        new preset("Anxiety", new float[]{2, 1, 1, 1.2f, 1, 1.4f, 1.2f, 1.1f, 1}),
        new preset("Canadian", new float[]{3, 8, 24, 4, 1.8f, 0, 0.7f, 0.9f, 1}),
        new preset("Big Chungus", new float[]{4, 5, 12, 1, 3, 0.2f, 1, 0.5f, 2}),
        new preset("[[BIG SHOT]]", new float[]{5, 6, 6, 0.5f, 10, 0.1f, 1.2f, 0.4f, 2}),
        new preset("Tempenyaki", new float[]{6, 3, 6, 1, 1, 1, 3f, 2f, 2f}),
        new preset("You paid for the entire speedometer", new float[]{7, 0.5f, 1.5f, 5, 1, 1.3f, 0.8f, 2, 1}),
        new preset("No Spawn Camp", new float[]{8, 0, 4, 1, 1, 1, 0.8f, 1, 1}),
    };
    //public int respawnTime = 0, turnTime = 1, speedMult = 2, scaleMult = 3, damageMult = 4, volume = 5, pitch = 6, attenuation = 7;
    public float[] shit = new float[9];

    public int charId = 0;
    int[][] selArr = new int[][] { 
        new int[] { 0 }, 
        new int[] { 1, 2, 3 },
        new int[] { 4, 5, 7 },
        new int[] { 6 },
        new int[] { 8 },
        new int[] { 9, 10 },
        new int[] { 11 },
    };

    float[] scalers = new float[]
    {
        1.8f, 0.8f, 0.8f, 0.8f, 1.7f, 1.7f, 1.3f, 1.7f, 1.8f, 2, 2, 1.4f
    };

    float[] translaters = new float[]
    {
        0, 0, 0, 0, -0.7f, -0.7f, -1.3f, -0.7f, -1.3f, -1.3f, -1.3f, -1.4f
    };
    /*
    float[][] scalers = new float[][] {
        new float[] { 1.8f },
        new float[] { 0.8f, 0.8f, 0.8f },
        new float[] { 1.7f, 1.7f, 1.7f },
        new float[] { 1.3f },
        new float[] { 1.8f },
        new float[] { 1.8f },
    };

    float[][] translaters = new float[][]
    {
        new float[] { 0 },
        new float[] { 0, 0, 0 },
        new float[] { -0.7f, -0.7f, -0.7f },
        new float[] { -1.3f },
        new float[] { -1.3f },
        new float[] { -1.3f }
    };*/

    int ii = 0;
    int jj = 0;
    int kk = 0;

    public GameObject[] textItems;

	void Start () {
        canvas = ui.GetComponent<Canvas>();
        sceneCam = GetComponent<Camera>();
        updateChar();
        updateText();
    }
	

    void updateChar()
    {
        ii = CarlMath.modClamp(ii, selArr.Length);
        jj = CarlMath.modClamp(jj, selArr[ii].Length);
        charId = selArr[ii][jj];
        GameObject.Destroy(tempModel);
        tempModel = Instantiate(playables[charId], charDisplay.transform.position, charDisplay.transform.rotation);
        tempModel.transform.SetParent(charDisplay.transform);
        if (tempModel.GetComponent<Character>().camAttach != null)
        {
            tempGun = Instantiate(tempModel.GetComponent<Character>().camAttach, tempModel.transform.position, tempModel.transform.rotation);
            tempGun.transform.SetParent(tempModel.transform);
        }
        charText.GetComponent<Text>().text = tempModel.GetComponent<Character>().title + "\n\nHP: " + tempModel.GetComponent<Character>().HP + "\nAC: " + (tempModel.GetComponent<Character>().AC + tempModel.GetComponent<Character>().ACMagicBonus) + "\nWalk Speed: " + tempModel.GetComponent<Character>().WALK_SPEED + "\nClimb Speed: " + tempModel.GetComponent<Character>().CLIMB_SPEED + "\nFly Speed: " + tempModel.GetComponent<Character>().FLY_SPEED + "\nHover: " + tempModel.GetComponent<Character>().HOVER;
        if (GameManager.instance != null)
            GameManager.instance.charId = charId;
        if (charId < scalers.Length)
            tempModel.transform.localScale *= scalers[charId];
        if (charId < translaters.Length)
            tempModel.transform.position += Vector3.up * translaters[charId];
        //Debug.Log(charId);
        if (GameManager.instance != null)
        {
            GameManager.instance.importShit();
        }
    }

    void updateText()
    {
        //Debug.Log("kk = " + kk);
        if (kk > 1)
            shit[kk-1] = Mathf.Round(shit[kk-1] * 10) * 0.1f;
        else if (kk == 1)
        {
            //shit[kk - 1] = Mathf.Round(shit[kk - 1]);
            textItems[0].GetComponent<Text>().text = textItems[0].GetComponent<Text>().text.Substring(0, textItems[0].GetComponent<Text>().text.IndexOf('-') + 2) + presets[(int)shit[kk-1]].name;
            shit = presets[(int)shit[0]].shit;
        }
        for (int i = 1; i < Mathf.Min(shit.Length, textItems.Length); i++)
        {
            textItems[i].GetComponent<Text>().text = shit[i] + " " + textItems[i].GetComponent<Text>().text.Substring(textItems[i].GetComponent<Text>().text.IndexOf('-'));
        }
        if (GameManager.instance != null)
        {
            GameManager.instance.importShit();
        }
    }

    //public LayerMask clickMask;

    // Update is called once per frame
    void Update () {

        /*if (Input.GetButton("Fire1"))
        {
            //Effects.instance.Sparky(transform.position, Input.mousePosition, null, null);
            //Debug.Log("Click at " + Input.mousePosition);
            Vector3 dir = new Vector3(-(-0.5f + Input.mousePosition.y / Screen.height) * sceneCam.fieldOfView, (-0.5f + Input.mousePosition.x/Screen.width) * sceneCam.fieldOfView, 0);
            Debug.Log("Click facing " + dir);
            //Effects.instance.Sparky(transform.position, )
            RaycastHit hit;
            //Vector3.Angle()
            //point.transform.localEulerAngles = dir;
            if (Physics.Raycast(transform.position, transform.localEulerAngles + dir, out hit, 50, clickMask))
            {
                if (hit.collider.tag == "Button")
                {
                    Debug.Log("Button Press");
                    hit.collider.GetComponent<Button>().onClick.Invoke();
                }
            }
        }*/

        if (tempModel.GetComponent<Character>().id == 8)
            tempModel.GetComponent<Character>().creature.specialAnim(1f);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (kk > 0)
                textItems[kk - 1].GetComponent<Text>().color = Color.white;
            kk++;
            kk %= 1 + Mathf.Min(shit.Length, textItems.Length);
            if (kk > 0)
                textItems[kk - 1].GetComponent<Text>().color = Color.red;
        } else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (kk > 0)
                textItems[kk - 1].GetComponent<Text>().color = Color.white;
            kk--;
            if (kk < 0)
                kk = Mathf.Min(shit.Length, textItems.Length);
            if (kk > 0)
                textItems[kk - 1].GetComponent<Text>().color = Color.red;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (kk == 0) {
                ii--;
                jj = 0;
                updateChar();
            } else if (kk == 1)
            {
                shit[kk - 1] = Mathf.Round(shit[kk-1]-1.4f);
                shit[kk - 1] %= presets.Length;
                while (shit[kk - 1] < 0)
                    shit[kk - 1] += presets.Length;
                updateText();
            } else
            {
                shit[kk-1] = Mathf.Max(shit[kk-1] - 0.1f, 0);
                updateText();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (kk == 0) { 
                ii++;
                jj = 0;
                updateChar();
            } else if (kk == 1)
            {
                shit[kk - 1] = Mathf.Round(shit[kk - 1]+1.4f);
                shit[kk - 1] %= presets.Length;
                updateText();
            } else
            {
                shit[kk-1]+=0.1f;
                updateText();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (kk == 0) { 
                jj++;
                updateChar();
            } else
            {
                //shit[kk - 1]+=0.1f;
                //updateText();
                if (kk > 0)
                    textItems[kk - 1].GetComponent<Text>().color = Color.white;
                kk--;
                if (kk < 0)
                    kk = Mathf.Min(shit.Length, textItems.Length);
                if (kk > 0)
                    textItems[kk - 1].GetComponent<Text>().color = Color.red;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (kk == 0) {
                jj--;
                updateChar();
            }
            else
            {
                //shit[kk - 1] = Mathf.Max(shit[kk - 1] - 0.1f, 0);
                //updateText();
                if (kk > 0)
                    textItems[kk - 1].GetComponent<Text>().color = Color.white;
                kk++;
                kk %= 1 + Mathf.Min(shit.Length, textItems.Length);
                if (kk > 0)
                    textItems[kk - 1].GetComponent<Text>().color = Color.red;
            }
        }

    }
}
