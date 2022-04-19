using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    [SyncVar]
    public int charId = 1;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject crosshair;

    [SerializeField]
    GameObject uiCanvas;

    //[SerializeField]
    public GameObject model;

    Vector3 pauseMenuScale;

    [SyncVar]
    private bool isDead = false;
    public bool dead
    {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    private float maxHP;
    public float maxHealth
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    [SyncVar]
    private float HP;
    public float health
    {
        get { return HP; }
        set { HP = value; }
    }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    //public KeyCode activationKey;
    private GameObject flashView;
    
    public GameObject[] playables;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private Camera cam3;

    public Character chara;

    public void Setup()
    {
        charId = GameManager.instance.charId;
        if (isLocalPlayer)
        {
            CmdLoadModel(charId);
            //CmdBroadcastCharId(charId);
        }
        else
            loadModel();

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Create Flash Panel
        flashView = new GameObject();
        flashView.name = "FlashCanvas";
        flashView.AddComponent<Canvas>();
        Canvas myCanvas = flashView.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        flashView.AddComponent<CanvasScaler>();
        flashView.AddComponent<GraphicRaycaster>();
        flashView.transform.SetParent(transform);
        GameObject panel = new GameObject("Panel");
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(2000, 2000);
        panel.transform.SetParent(myCanvas.transform, false);
        flashView.SetActive(false);


        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    [Command]
    public void CmdBroadcastCharId(int id)
    {
        RpcBroadcastCharId(id);
    }

    [ClientRpc]
    public void RpcBroadcastCharId(int id)
    {
        this.charId = id;
    }

    [Command]
    public void CmdLoadModel(int id)
    {
        //loadModel();
        RpcLoadModel(id);
    }

    [ClientRpc]
    public void RpcLoadModel(int id)
    {
        charId = id;
        loadModel();
    }

    public void loadModel()
    {
        if (model == null)
        {
            Debug.Log("Instantiating Player Model");
            model = Instantiate(playables[charId], transform.position, transform.rotation);
            chara = model.GetComponent<Character>();
            model.transform.localPosition += chara.offset;
            model.transform.localEulerAngles += chara.rotate;
            model.transform.parent = transform;
            if (chara.camAttach != null)
            {
                chara.camAttach = Instantiate(chara.camAttach, cam.transform.position, cam.transform.rotation);
                chara.camAttach.transform.parent = cam.transform;
                chara.primaryEmitters = arrayCombine(chara.primaryEmitters, chara.camAttach.GetComponent<CamAttach>().primaryEmitters);
                chara.secondaryEmitters = arrayCombine(chara.secondaryEmitters, chara.camAttach.GetComponent<CamAttach>().secondaryEmitters);
                chara.tertiaryEmitters = arrayCombine(chara.tertiaryEmitters, chara.camAttach.GetComponent<CamAttach>().tertiaryEmitters);
                chara.canJumpFrom = arrayCombine(chara.canJumpFrom, chara.camAttach.GetComponent<CamAttach>().canJumpFrom);
            }
            cam.fieldOfView = chara.fov;
            //GetComponents<NetworkTransformChild>()[1].target = model.transform;
            if (isLocalPlayer)
            {
                foreach (GameObject e in chara.hideFirstPerson)
                    e.layer = 11;
                foreach (GameObject e in chara.hideThirdPerson)
                    e.layer = 12;
            } else
            {
                foreach (GameObject e in chara.hideFirstPerson)
                    e.layer = 9;
                foreach (GameObject e in chara.hideThirdPerson)
                    e.layer = 10;
            }
            GetComponent<Rigidbody>().useGravity = !chara.HOVER;
            GetComponent<PlayerController>().Setup();
            model.transform.name = transform.name;
            GetComponent<PlayerShoot>().Init(chara);
        }
        maxHP = chara.HP;
        health = maxHP;
    }

    private GameObject[] arrayCombine(GameObject[] arr0, GameObject[] arr1)
    {
        GameObject[] result = new GameObject[arr0.Length + arr1.Length];

        for (int i = 0; i < arr0.Length; i++)
            result[i] = arr0[i];

        for (int i = 0; i < arr1.Length; i++)
            result[arr0.Length + i] = arr1[i];

        return result;
    }

    public GameObject getPrimaryEmitter()
    {
        return chara.primaryEmitters[Random.Range(0, chara.primaryEmitters.Length)];
    }

    public GameObject getSecondaryEmitter()
    {
        return chara.secondaryEmitters[Random.Range(0, chara.secondaryEmitters.Length)];
    }

    public GameObject getTertiaryEmitter()
    {
        return chara.tertiaryEmitters[Random.Range(0, chara.tertiaryEmitters.Length)];
    }

    [Command]
    public void CmdRotate(Vector3 amount)
    {
        RpcRotate(amount);
    }

    [ClientRpc]
    public void RpcRotate(Vector3 amount)
    {
        if (!isLocalPlayer)
        {
            if (model == null)
            {
                loadModel();
            }
            model.transform.eulerAngles = amount;
        }
    }

    [Command]
    public void CmdTakeDamage(float amount)
    {
        RpcTakeDamage(amount);
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount)
    {
        if (dead)
            return;

        if (isLocalPlayer)
            StartCoroutine("PlayFlashAnimation");
        else
            StartCoroutine(DamageFlash());

        HP -= amount * GameManager.instance.matchSettings.damageMult;

        //Debug.Log(transform.name + " now has " + HP + " HP");

        if (HP <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        foreach (GameObject e in chara.tintable)
            e.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 1f);
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject e in chara.tintable)
            e.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
    }

    IEnumerator PlayFlashAnimation()
    {
       
        //Debug.Log("Show Flash Damage Animation");

        if (!flashView.activeSelf)
        {
            flashView.SetActive(true);
        }
        Image img = flashView.GetComponentInChildren<Image>();
        if (img != null)
        {
            for (int i = 0; i < 6; i+=1)
            {
                img.color = new Color(1f, 0f, 0f, 0.5f-i*0.1f);
                yield return new WaitForSeconds(0.02f);
            }
        }
        flashView.SetActive(false);
        /*
        LeanTween.value(flashView, 1.0f, 0.0f, 1f)
            .setOnUpdate((float val) => {
                if (img != null)
                {
                    img.color = new Color(targetColor.r, targetColor.g, targetColor.b, val);
                }
            })
            .setOnComplete(() => {
                flashView.SetActive(false);
            });*/
        //return null;
    }

    private void Die()
    {
        dead = true;

        //DISABLE COMPONENTS

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Debug.Log(transform.name + " is DEAD!");

        /*
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;*/

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    public void SetDefaults()
    {
        dead = false;

        HP = maxHP;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        /*
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;*/
    }

    private void Start()
    {
        PauseGame.IsOn = false;
        //pauseMenu.GetComponent<CanvasRenderer>().SetAlpha(1f);
        //pauseMenu.GetComponent<CanvasGroup>().alpha = 1f;
        //pauseMenuScale = pauseMenu.transform.localScale;
        //GameObject.Destroy(pauseMenu);
        Debug.Log("Starting");
    }

    public void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (isLocalPlayer && Input.GetKeyDown(KeyCode.K))
        {
            CmdTakeDamage(99999999999f);
        }

        if (isLocalPlayer && Input.GetKeyDown(KeyCode.L))
        {
            CmdTakeDamage(10f);
        }
    }

    GameObject pauseMenuInstance;

    public void TogglePauseMenu()
    {
        if (!PauseGame.IsOn) {
            //crosshair.GetComponent<CanvasRenderer>().SetAlpha(1f);
            //pauseMenu.transform.localScale = pauseMenuScale;
            //pauseMenuInstance = Instantiate(pauseMenu) as GameObject;
            PauseGame.IsOn = true;
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        } else
        {
            //crosshair.GetComponent<CanvasRenderer>().SetAlpha(0);
            /*if (pauseMenu != null)
                GameObject.Destroy(pauseMenuInstance);*/
            PauseGame.IsOn = false;
            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
