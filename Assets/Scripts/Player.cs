using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    [SyncVar]
    public int charId = -1;

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

    [SerializeField]
    private LayerMask jumpMask;

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

    private bool deathTint = false;

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

    float walkSpeed;
    float climbSpeed;
    float flySpeed;
    float jumpForce;

    private const float speedMult = 3.15f;

    public void Setup()
    {


        if (isLocalPlayer)
        {
            charId = GameManager.instance.charId;
            CmdLoadModel(charId);
            //CmdBroadcastCharId(charId);
        }
        //else
        //    loadModel();

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
        if (!isLocalPlayer)
            charId = id;
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
        if (model == null && charId != -1)
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
                cam.transform.localPosition = chara.cameraOffset;
            } else
            {
                foreach (GameObject e in chara.hideFirstPerson)
                    e.layer = 9;
                foreach (GameObject e in chara.hideThirdPerson)
                    e.layer = 10;
                model.layer = 9;
            }
            GetComponent<Rigidbody>().useGravity = !chara.HOVER;

            walkSpeed = speedMult * (chara.WALK_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult * 1.17f * 1.55f;
            climbSpeed = speedMult * (chara.CLIMB_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult;
            flySpeed = 1.383f*speedMult * (chara.FLY_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult;
            jumpForce = chara.jumpHeight * GameManager.instance.matchSettings.moveSpeedMult * 10f;

            GetComponent<PlayerController>().Setup(walkSpeed, climbSpeed, flySpeed, jumpForce);
            model.transform.name = transform.name;
            GetComponent<PlayerShoot>().Init(chara);

            //if (isLocalPlayer && chara.camAttachTo != null)
            //{
            //    cam.transform.SetParent(chara.camAttachTo.transform);
            //}
        }
        //else if (charId == -1 && !isLocalPlayer)
        //    CmdQueryId();
        if (chara != null)
        {

            maxHP = chara.HP;
            health = maxHP;
        }
    }

    [Command]
    private void CmdQueryId()
    {
        RpcQueryId();
    }

    [ClientRpc]
    private void RpcQueryId()
    {
        if (charId != -1)
            CmdSendId(charId);
    }

    [Command]
    private void CmdSendId(int id)
    {
        RpcSendId(id);
    }

    [ClientRpc]
    private void RpcSendId(int id)
    {
        charId = id;
        loadModel();
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
        chara.creature.damage(amount);
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
        deathTint = false;
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
        foreach (GameObject e in chara.tintable)
            e.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 1f);

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

        if (chara.camAttachTo == null) GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

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
        newEulers = 0f;
        if (chara != null)
            cam.transform.eulerAngles =chara.camAttachTo.transform.eulerAngles;

        dead = false;

        HP = maxHP;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        if (chara != null)
            foreach (GameObject e in chara.tintable)
                e.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);

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

    private Vector3 MaxV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return a;
        else return b;
        //return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
    }

    private Vector3 MinV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return b;
        else return a;
        //return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
    }

    float newEulers = 0f;
    //float targetEulers;

    public void Update()
    {
        if (isLocalPlayer && chara != null && chara.camAttachTo != null)
        {
            //cam.transform.position = RotatePointAroundPivot(chara.camAttachTo.transform.position, transform.position, chara.rotate);
            cam.transform.position += MinV(chara.camAttachTo.transform.position + chara.cameraOffset.z * chara.camAttachTo.transform.forward + chara.cameraOffset.x*chara.camAttachTo.transform.right + chara.cameraOffset.y * chara.camAttachTo.transform.up - cam.transform.position, 0.1f*(chara.camAttachTo.transform.position + chara.cameraOffset.z * chara.camAttachTo.transform.forward + chara.cameraOffset.x * chara.camAttachTo.transform.right + chara.cameraOffset.y * chara.camAttachTo.transform.up - cam.transform.position).normalized);
            if (Mathf.Abs(chara.camAttachTo.transform.eulerAngles.z - cam.transform.eulerAngles.z) < 60f) {
                cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, chara.camAttachTo.transform.eulerAngles.z);
                ////targetEulers = chara.camAttachTo.transform.eulerAngles.z;
                //newEulers += Mathf.Min(chara.camAttachTo.transform.eulerAngles.z - newEulers, 0.05f);
                //cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, newEulers);
            }
            //cam.transform.SetParent(chara.camAttachTo.transform);
        }
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

        if ((isDead || health < 0) && !deathTint)
        {
            deathTint = true;
            if (chara != null)
                foreach (GameObject e in chara.tintable)
                    e.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 1f);
        } else if (health > 0 && deathTint)
        {
            deathTint = false;
            if (chara != null)
                foreach (GameObject e in chara.tintable)
                    e.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
        }
        if (chara != null && GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            float vel = GetComponent<Rigidbody>().velocity.magnitude;
            if (IsGrounded() && chara.WALK_SPEED > 0)
            {
                chara.creature.walkAnim(Mathf.Clamp(2*vel / walkSpeed, 0.1f, 3f));
                if (!chara.HOVER)
                    GetComponent<Rigidbody>().useGravity = true;
            } else if (chara.FLY_SPEED > 0)
            {
                //Debug.Log("gliding");
                chara.creature.flyAnim(Mathf.Clamp(vel / flySpeed, 0.8f, 3f));
                if (!chara.HOVER)
                    GetComponent<Rigidbody>().useGravity = Mathf.Sqrt(Mathf.Pow(GetComponent<Rigidbody>().velocity.x,2) + Mathf.Pow(GetComponent<Rigidbody>().velocity.z, 2)) / flySpeed < 0.8f;
                //Debug.Log(GetComponent<Rigidbody>().velocity);
                //Debug.Log(Mathf.Abs(GetComponent<Rigidbody>().velocity.z) / flySpeed > 0.8f);
            }
        } else if (chara != null)
        {
            chara.creature.idleAnim();
            if (!chara.HOVER)
                GetComponent<Rigidbody>().useGravity = true;
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

    public bool IsGrounded()
    {
        RaycastHit hit;
        //int i = 0;
        foreach (GameObject e in chara.canJumpFrom)
            if (e.GetComponent<SphereCollider>() != null)
            {
                //Effects.instance.CmdSparky(e.transform.position+e.GetComponent<SphereCollider>().center, e.transform.position - Vector3.up*(e.GetComponent<SphereCollider>().radius + 0.1f), null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<SphereCollider>().center, -Vector3.up, out hit, e.GetComponent<SphereCollider>().radius * e.GetComponent<SphereCollider>().transform.localScale.magnitude + 0.1f, jumpMask);
                //if (yeet)
                //    Debug.Log("Touching Ground");
                if (yeet)
                    return yeet;
            }
            else if (e.GetComponent<CapsuleCollider>() != null)
            {
                //Effects.instance.CmdSparky(e.transform.position + e.GetComponent<CapsuleCollider>().center, e.transform.position - Vector3.up * (0.25f * e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.1f), null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<CapsuleCollider>().center, -Vector3.up, out hit, 0.25f * e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.1f, jumpMask);
                //if (yeet)
                //    Debug.Log("Touching Ground");
                if (yeet)
                    return yeet;
            }
        //else if (e.GetComponent<SphereCollider>() != null)
        //    return Physics.Raycast(e.GetComponent<SphereCollider>().center, -Vector3.up, e.GetComponent<SphereCollider>().radius + 0.1f);
        return false;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
