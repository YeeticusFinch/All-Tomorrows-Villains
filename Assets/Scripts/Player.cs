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
    public GameObject crosshair;

    [SerializeField]
    public GameObject uiCanvas;

    [SerializeField]
    public GameObject healthText;
    [SerializeField]
    public GameObject infoText;

    [SerializeField]
    public GameObject healthText3;
    [SerializeField]
    public GameObject infoText3;

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

    /*private struct Jumpable
    {
        public GameObject obj;
        public bool grounded;
        public Jumpable(GameObject obj)
        {
            this.obj = obj;
            grounded = false;
        }
    }

    private Jumpable[] jumpables; */

    //private List<GameObject> jumpables = new List<GameObject>();

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
    public Camera cam3;

    public Character chara;

    float walkSpeed;
    float climbSpeed;
    float flySpeed;
    float jumpForce;

    private const float speedMult = 3.15f;

    //[SerializeField]
    //private GameObject healthBar;

    public void Setup()
    {

        playables = GameManager.instance.playables;
        if (!isLocalPlayer)
        {
            healthBarPlzWork.enabled = false;
            healthBarPlzWork2.enabled = false;
        } else
        {
            charId = GameManager.instance.charId;
            CmdLoadModel(charId);
            //CmdBroadcastCharId(charId);
            GameManager.localPlayer = this;
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

    public bool IsGrounded()
    {
        return grounded;
        foreach (GameObject e in chara.canJumpFrom)
            if (e.GetComponent<GroundControl>().IsGrounded())
                return true;
        return false;
    }

    public bool grounded = false;
    const float GROUND_THRESHOLD = 0.5f;
    public Vector3 groundContactPos;

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("GroundControl touching collider");
        if (!grounded)
            foreach (ContactPoint c in collision.contacts)
                if (Vector3.Dot(c.normal, Vector3.up) > GROUND_THRESHOLD)
                {
                    grounded = true;
                    groundContactPos = c.point;
                }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("GroundControl hit collider");
        if (!grounded)
        {
            foreach (ContactPoint c in collision.contacts)
            {
                Debug.Log("c.normal enter = " + c.normal);
                if (Vector3.Dot(c.normal, Vector3.up) > GROUND_THRESHOLD)
                {
                    grounded = true;
                    groundContactPos = c.point;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("GroundControl left collider");
        grounded = false;
        if (grounded)
        {
            Debug.Log("Still grounded reee");
            foreach (ContactPoint c in collision.contacts)
            {
                Debug.Log("c.normal exit = " + c.normal);
                if (Vector3.Dot(c.normal, Vector3.up) > GROUND_THRESHOLD)
                    grounded = false;
            }
        }
    }

    public void loadModel()
    {
        if (model == null && charId != -1)
        {
            //Debug.Log("CharID: " + charId);
            transform.localScale = (new Vector3(1, 1, 1)) * GameManager.instance.matchSettings.scaleMult;
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
                chara.primaryEmitters = CarlMath.arrayCombine(chara.primaryEmitters, chara.camAttach.GetComponent<CamAttach>().primaryEmitters);
                chara.secondaryEmitters = CarlMath.arrayCombine(chara.secondaryEmitters, chara.camAttach.GetComponent<CamAttach>().secondaryEmitters);
                chara.tertiaryEmitters = CarlMath.arrayCombine(chara.tertiaryEmitters, chara.camAttach.GetComponent<CamAttach>().tertiaryEmitters);
                chara.canJumpFrom = CarlMath.arrayCombine(chara.canJumpFrom, chara.camAttach.GetComponent<CamAttach>().canJumpFrom);
            }
            /*foreach (GameObject e in chara.canJumpFrom)
            {
                //jumpables.Add(GameObject.Instantiate(new GameObject()));
                //jumpables[jumpables.Count - 1].AddComponent<GroundControl>();
                //jumpables[jumpables.Count - 1].AddComponent<Collider>();
                //jumpables[jumpables.Count - 1].GetComponent<Collider>(). = e.GetComponent<Collider>();
                //e.AddComponent<GroundControl>();
                //e.GetComponent<GroundControl>().player = this;
            }*/
            cam.fieldOfView = chara.fov;
            cam3.fieldOfView = chara.fov;
            float distOff = 0.2f;
            if (isLocalPlayer)
            {
                healthText.transform.localPosition = cam.transform.forward * 0.2f * distOff + cam.transform.up * 0.1f * distOff * cam.fieldOfView / 60;
                infoText.transform.localPosition = cam.transform.forward * 0.2f * distOff + (cam.transform.up*1.1f + cam.transform.right) * 0.1f * distOff * cam.fieldOfView / 60;

                healthText3.transform.localPosition = cam3.transform.forward * 0.2f * distOff + cam3.transform.up * 0.1f * distOff * cam3.fieldOfView / 60;
                infoText3.transform.localPosition = cam3.transform.forward * 0.2f * distOff + (cam3.transform.up * 1.1f + cam3.transform.right) * 0.1f * distOff * cam3.fieldOfView / 60;
            }
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

            walkSpeed = speedMult * (chara.WALK_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult * 1.17f/* * 1.55f * 0.682f*/*1.055f;
            climbSpeed = speedMult * (chara.CLIMB_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult;
            flySpeed = /*1.383f**/speedMult * (chara.FLY_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult*0.74f;
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

    public float GetSpeed()
    {
        return GetComponent<PlayerMotor>().rb.velocity.magnitude * 81f * 1.221f / (/*2.4533f * */speedMult * GameManager.instance.matchSettings.speedMult * 1.17f * 1.55f);
        //return GetComponent<PlayerMotor>().rb.velocity.magnitude * speedMult * (chara.WALK_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult * 1.17f * 1.55f;
        //return 0;
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
    public void CmdTakeDamage(float amount, string dmgType, int num, string save)
    {
        RpcTakeDamage(amount, dmgType, num, save);
    }

    [ClientRpc]
    public void RpcTakeDamage(float amount, string dmgType, int num, string save)
    {
        if (dead)
            return;

        if (isLocalPlayer)
            StartCoroutine("PlayFlashAnimation");
        else
            StartCoroutine(DamageFlash());
        if (save == null && chara != null)
            amount *= Mathf.Clamp((19 - chara.AC + num) / 20f, 0.001f, 1); // Average damage taking 'to hit' and 'AC' into account
        else if (chara != null)
            amount *= Mathf.Clamp(0.5f + 0.5f * (num - chara.GetSave(save)) / 20f, 0.5f, 1); // Average damage for saving throws
        HP -= amount * chara.GetDamageMod(dmgType) * GameManager.instance.matchSettings.damageMult;
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
            foreach (Material m in e.GetComponent<Renderer>().materials)
                m.color = new Color(1f, 0f, 0f, 1f);
        yield return new WaitForSeconds(0.1f);
        foreach (GameObject e in chara.tintable)
            foreach (Material m in e.GetComponent<Renderer>().materials)
                m.color = new Color(1f, 1f, 1f, 1f);
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
            foreach (Material m in e.GetComponent<Renderer>().materials)
                m.color = new Color(1f, 0f, 0f, 1f);

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

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        SetDefaults();
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    public void SetDefaults()
    {
        newEulers = 0f;
        if (chara != null && chara.camAttachTo != null && chara.camAttachToRotate)
            cam.transform.eulerAngles = chara.camAttachTo.transform.eulerAngles;

        dead = false;

        HP = maxHP;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        if (chara != null)
            foreach (GameObject e in chara.tintable)
                foreach (Material m in e.GetComponent<Renderer>().materials)
                    m.color = new Color(1f, 1f, 1f, 1f);

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
        GameManager.instance.isServer = isServer;
    }

    float newEulers = 0f;
    //float targetEulers;
    private int bloodied = -1;
    public void Update()
    {
        if (isLocalPlayer && chara != null && chara.camAttachTo != null)
        {
            //cam.transform.position = RotatePointAroundPivot(chara.camAttachTo.transform.position, transform.position, chara.rotate);
            cam.transform.position += CarlMath.MinV(chara.camAttachTo.transform.position + chara.cameraOffset.z * chara.camAttachTo.transform.forward + chara.cameraOffset.x*chara.camAttachTo.transform.right + chara.cameraOffset.y * chara.camAttachTo.transform.up - cam.transform.position, 0.1f*(chara.camAttachTo.transform.position + chara.cameraOffset.z * chara.camAttachTo.transform.forward + chara.cameraOffset.x * chara.camAttachTo.transform.right + chara.cameraOffset.y * chara.camAttachTo.transform.up - cam.transform.position).normalized);
            if (chara.camAttachToRotate && Mathf.Abs(chara.camAttachTo.transform.eulerAngles.z - cam.transform.eulerAngles.z) < 60f) {
                //cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, (Mathf.Abs(chara.camAttachTo.transform.eulerAngles.z-180-GetComponent<PlayerMotor>().roll) < 10  ? 180 : 0)+chara.camAttachTo.transform.eulerAngles.z);
                cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, chara.camAttachTo.transform.eulerAngles.z + GetComponent<PlayerMotor>().roll);
                //Debug.Log("rot = " + chara.camAttachTo.transform.eulerAngles + " roll = " + GetComponent<PlayerMotor>().roll);
                ////targetEulers = chara.camAttachTo.transform.eulerAngles.z;
                //newEulers += Mathf.Min(chara.camAttachTo.transform.eulerAngles.z - newEulers, 0.05f);
                //cam.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, newEulers);
            }
            //cam.transform.SetParent(chara.camAttachTo.transform);
        }
        if (chara != null && chara.spinWithCamera != null && chara.spinWithCamera.Length > 0)
        {
            foreach(GameObject e in chara.spinWithCamera)
            {
                //e.transform.localEulerAngles = (cam.transform.localEulerAngles - e.transform.localEulerAngles) * chara.spinWithCameraScalar;
                e.transform.eulerAngles = cam.transform.eulerAngles;
            }
        }
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }

        if (isLocalPlayer && Input.GetKeyDown(KeyCode.K))
        {
            CmdTakeDamage(99999999999f, "force", 20, null);
        }

        if (isLocalPlayer && Input.GetKeyDown(KeyCode.L))
        {
            CmdTakeDamage(10f, "force", 20, null);
        }

        if ((isDead || health < 0) && !deathTint)
        {
            deathTint = true;
            if (chara != null)
                foreach (GameObject e in chara.tintable)
                    foreach (Material m in e.GetComponent<Renderer>().materials)
                        m.color = new Color(1f, 0f, 0f, 1f);
        } else if (health > 0 && deathTint)
        {
            deathTint = false;
            if (chara != null)
                foreach (GameObject e in chara.tintable)
                    foreach (Material m in e.GetComponent<Renderer>().materials)
                        m.color = new Color(1f, 1f, 1f, 1f);
        }
        if (chara != null && GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            float vel = GetComponent<Rigidbody>().velocity.magnitude;
            if (IsGrounded() && chara.WALK_SPEED > 0)
            {
                chara.creature.walkAnim(Mathf.Clamp(2*vel / walkSpeed, 0.1f, 3f), GetComponent<Rigidbody>().velocity.normalized);
                if (!chara.HOVER)
                    GetComponent<Rigidbody>().useGravity = true;
            } else if (chara.FLY_SPEED > 0)
            {
                //Debug.Log("gliding");
                chara.creature.flyAnim(Mathf.Clamp(vel / flySpeed, 0.8f, 3f));
                if (!chara.HOVER) {
                    Vector3 v = GetComponent<Rigidbody>().velocity;
                    GetComponent<Rigidbody>().useGravity = Mathf.Sqrt(Mathf.Pow(v.x, 2) + (v.y > 0 ? v.y*0.4f : 0) + Mathf.Pow(v.z, 2)) / flySpeed < 0.6f;
                }
                //Debug.Log(GetComponent<Rigidbody>().velocity);
                //Debug.Log(Mathf.Abs(GetComponent<Rigidbody>().velocity.z) / flySpeed > 0.8f);
            } else
            {
                chara.creature.fallAnim(Mathf.Clamp(2 * vel / walkSpeed, 0.1f, 3f), GetComponent<Rigidbody>().velocity.normalized);
            }
        } else if (chara != null)
        {
            chara.creature.idleAnim();
            if (!chara.HOVER)
                GetComponent<Rigidbody>().useGravity = true;
        }
        if (isLocalPlayer)
        {
            //healthBar.GetComponent<bar>().setPos(health / maxHealth);
            Camera currCam = cam.enabled ? cam : cam3;
            float distOff = 0.2f * 60 / currCam.fieldOfView;
            healthBarPlzWork.SetPosition(0, currCam.transform.position + currCam.transform.forward*0.2f* distOff + currCam.transform.up * 0.1f* distOff * currCam.fieldOfView/60 - currCam.transform.right * 0.1f* distOff * currCam.fieldOfView / 60 * (maxHP/150)* Mathf.Max(0, HP)/maxHP);
            healthBarPlzWork.SetPosition(1, currCam.transform.position + currCam.transform.forward*0.2f* distOff + currCam.transform.up * 0.1f* distOff * currCam.fieldOfView / 60 + currCam.transform.right * 0.1f* distOff * currCam.fieldOfView / 60 * (maxHP / 150) * Mathf.Max(0, HP)/maxHP);
            healthBarPlzWork2.SetPosition(0, currCam.transform.position + currCam.transform.forward * 0.21f* distOff + currCam.transform.up * 0.105f* distOff * currCam.fieldOfView / 60 - currCam.transform.right* distOff * currCam.fieldOfView / 60 * (maxHP / 150) * 0.105f);
            healthBarPlzWork2.SetPosition(1, currCam.transform.position + currCam.transform.forward * 0.21f* distOff + currCam.transform.up * 0.105f* distOff * currCam.fieldOfView / 60 + currCam.transform.right* distOff * currCam.fieldOfView / 60 * (maxHP / 150) * 0.105f);

            //distOff *= currCam.fieldOfView / 60;
            if (cam.enabled)
            {
                healthText.transform.localPosition = Vector3.forward * 0.2f * distOff + Vector3.up * 0.1f * distOff * cam.fieldOfView / 60;
                infoText.transform.localPosition = Vector3.forward * 0.2f * distOff + (Vector3.up * 1.1f + Vector3.right) * 0.1f * distOff * cam.fieldOfView / 60;
                healthText.GetComponent<TextMesh>().text = Mathf.RoundToInt(HP) + " / " + maxHP;
            } else
            {
                healthText3.transform.localPosition = Vector3.forward * 0.2f * distOff + Vector3.up * 0.1f * distOff * cam3.fieldOfView / 60;
                infoText3.transform.localPosition = Vector3.forward * 0.2f * distOff + (Vector3.up * 1.1f + Vector3.right) * 0.1f * distOff * cam3.fieldOfView / 60;
                healthText3.GetComponent<TextMesh>().text = Mathf.RoundToInt(HP) + " / " + maxHP;
            }
            //healthText.transform.position 
            if (HP > maxHP / 2 && bloodied != 0)
            {
                healthBarPlzWork.startColor = Color.green;
                healthBarPlzWork.endColor = Color.green;
                healthBarPlzWork.material.color = Color.green;
                //healthBarPlzWork.material. = Color.green;
                healthBarPlzWork.numCapVertices = 1;
                bloodied = 0;
            } else if (HP > maxHP/4 && HP < maxHP/2 && bloodied != 1)
            {
                healthBarPlzWork.startColor = Color.yellow;
                healthBarPlzWork.endColor = Color.yellow;
                healthBarPlzWork.material.color = Color.yellow;
                healthBarPlzWork.numCapVertices = 0;
                bloodied = 1;
            }
            else if (HP < maxHP/4 && bloodied != 2)
            {
                healthBarPlzWork.startColor = Color.red;
                healthBarPlzWork.endColor = Color.red;
                healthBarPlzWork.material.color = Color.red;
                healthBarPlzWork.numCapVertices = 0;
                bloodied = 2;
            }
        }
    }
    public LineRenderer healthBarPlzWork;
    public LineRenderer healthBarPlzWork2;
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

    /*public bool IsGrounded()
    {
        RaycastHit hit;
        //int i = 0;
        foreach (GameObject e in chara.canJumpFrom)
            if (e.GetComponent<SphereCollider>() != null)
            {
                //Effects.instance.CmdSparky(e.transform.position+e.GetComponent<SphereCollider>().center, e.transform.position - Vector3.up*(e.GetComponent<SphereCollider>().radius + 0.1f), null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<SphereCollider>().center, -Vector3.up, out hit, e.GetComponent<SphereCollider>().radius * e.GetComponent<SphereCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.01f, jumpMask);
                //if (yeet)
                //    Debug.Log("Touching Ground");
                if (yeet)
                    return yeet;
            }
            else if (e.GetComponent<CapsuleCollider>() != null)
            {
                //Effects.instance.CmdSparky(e.transform.position + e.GetComponent<CapsuleCollider>().center, e.transform.position - Vector3.up * (0.25f * e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.1f), null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<CapsuleCollider>().center, -Vector3.up, out hit, 0.25f * e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.01f, jumpMask);
                //if (yeet)
                //    Debug.Log("Touching Ground");
                if (yeet)
                    return yeet;
            }
        //else if (e.GetComponent<SphereCollider>() != null)
        //    return Physics.Raycast(e.GetComponent<SphereCollider>().center, -Vector3.up, e.GetComponent<SphereCollider>().radius + 0.1f);
        return false;
    }*/

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
