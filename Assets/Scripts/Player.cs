using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject crosshair;

    [SerializeField]
    GameObject uiCanvas;

    Vector3 pauseMenuScale;

    [SyncVar]
    private bool isDead = false;
    public bool dead
    {
        get { return isDead; }
        protected set { isDead = value; }
    }

    [SerializeField]
    private float maxHP = 100f;
    public float maxHealth
    {
        get { return maxHP; }
        set { maxHP = value; }
    }

    [SyncVar]
    private float HP = 100f;
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

    public void Setup()
    {

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

    [ClientRpc]
    public void RpcTakeDamage(float amount)
    {
        if (dead)
            return;

        if (isLocalPlayer)
            StartCoroutine("PlayFlashAnimation");

        HP -= amount;

        Debug.Log(transform.name + " now has " + HP + " HP");

        if (HP <= 0)
        {
            Die();
        }
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
            RpcTakeDamage(99999999999f);
        }

        if (isLocalPlayer && Input.GetKeyDown(KeyCode.L))
        {
            RpcTakeDamage(10f);
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
