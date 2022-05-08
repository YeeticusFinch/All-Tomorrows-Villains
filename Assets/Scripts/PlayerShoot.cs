
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    //[SerializeField]
    //public AudioSource shoot;

    private const string PLAYER_TAG = "Player";

    // Weapon selected
    public int selectedWeapon = 0;

    //public PlayerWeapon weapon;

    [SerializeField]
    public Camera cam;

    [SerializeField]
    public LayerMask mask;

    //[SerializeField]
    //private GameObject emitter;

    [SerializeField]
    private GameObject player;

    private bool canShoot = true;

    public Creature creature;

    public TextMesh infoText;
    public TextMesh infoText3;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
        if (isLocalPlayer)
        {
            camMode = 2;
            GetComponent<Player>().cam3.enabled = false;
            cam.enabled = true;
            GameManager.instance.thirdPerson = false;
            GameManager.instance.freeCam = false;
            infoText = GetComponent<Player>().infoText.GetComponent<TextMesh>();
            infoText3 = GetComponent<Player>().infoText3.GetComponent<TextMesh>();
        }
    }

    public void Init(Character chara)
    {
        creature = chara.creature;
        creature.mask = mask;
        creature.cam = cam;
        //creature.weapon = weapon;
        creature.player = player;
        infoText.text = "Initializing loadout...";
        if (GetComponent<PlayerController>())
            GetComponent<PlayerController>().creature = creature;
    }
    int camMode = 2;
    bool swapped = false;
    private void Update()
    {
        if (isLocalPlayer)
        {
            float deltaSelect = Input.GetAxisRaw("Swap Weapon");
            deltaSelect += Input.GetAxisRaw("Mouse ScrollWheel");
            if (deltaSelect != 0 && !swapped)
            {
                swapped = true;
                selectedWeapon += (int)Mathf.Sign(deltaSelect);
                selectedWeapon %= creature.attacks.Length;
                if (selectedWeapon < 0)
                    selectedWeapon += creature.attacks.Length;
                SetInfoText();
            } else if (deltaSelect == 0)
                swapped = false;

            if (Input.GetButtonDown("3rd Person"))
            {
                camMode = (camMode + 1) % 3;
                if (camMode == 0)
                {
                    cam.enabled = false;
                    GetComponent<Player>().cam3.enabled = true;
                    GameManager.instance.thirdPerson = true;
                    GameManager.instance.freeCam = false;
                    //GetComponent<Player>().crosshair.GetComponent<Sprite>().active = false;
                } else if (camMode == 1)
                {
                    cam.enabled = false;
                    GetComponent<Player>().cam3.enabled = true;
                    GameManager.instance.thirdPerson = true;
                    GameManager.instance.freeCam = true;
                    //GameObject.Destroy(GetComponent<Player>().crosshair);
                } else
                {
                    GetComponent<Player>().cam3.enabled = false;
                    cam.enabled = true;
                    GameManager.instance.thirdPerson = false;
                    GameManager.instance.freeCam = false;
                    //GetComponent<Player>().crosshair.GetComponent<RectTransform>().SetPositionAndRotation(new Vector2(0, 0), GetComponent<Player>().crosshair.GetComponent<RectTransform>().rotation);
                }
            }

            if (PauseGame.IsOn)
                return;

            if (Input.GetButton("Fire1") && canShoot)
            {
                //Debug.Log("1");
                canShoot = false;
                CmdPrimary(selectedWeapon);
                //Shoot();
            }
            if (Input.GetButton("Ability1"))
            {
                CmdAbility(canShoot, 0);
            }
            if (Input.GetButton("Ability2"))
            {
                CmdAbility(canShoot, 1);
            }
            if (Input.GetButtonDown("Fire2"))
            {
                creature.aim(1/3f);
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                creature.aim(3);
            }
        }
    }

    [Command]
    private void CmdPrimary(int selected)
    {
        //Debug.Log("3");
        RpcPrimary(selected);
    }

    [ClientRpc]
    private void RpcPrimary(int selected)
    {
        //Debug.Log("4");
        float x = creature.primary(selected);
        if (isLocalPlayer)
            StartCoroutine(ResetFire(x));
    }

    [Command]
    private void CmdAbility(bool canShoot, int n)
    {
        RpcAbility(canShoot, n);
    }

    [ClientRpc]
    private void RpcAbility(bool canShoot, int n)
    {
        float x = creature.ability(canShoot, n);
        if (isLocalPlayer && x > 0)
        {
            StartCoroutine(ResetFire(x));
        }
    }

    /*
    [Client]
    private void Shoot()
    {
        GameObject emitter = GetComponent<Player>().getPrimaryEmitter();
        RaycastHit hit;
        Effects.instance.CmdSparky(emitter.transform.position, emitter.transform.position + 0.03f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            Effects.instance.CmdSparky(emitter.transform.position, hit.point, shootSound, hitSound);
            Effects.instance.CmdSparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            Effects.instance.CmdSparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            Effects.instance.CmdSparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            // We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, weapon.damage);
            }
        } else
        {
            Effects.instance.CmdSparky(emitter.transform.position, emitter.transform.position + cam.transform.forward * weapon.range, shootSound, null);
        }
    }
    */
    [Command]
    public void CmdPlayerShot(string playerId, float damage, string dmgType, int num, string save)
    {
        //Debug.Log(playerId + " has been shot.");
        //Debug.Log("5");
        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage, dmgType, num, save);
    }

    public void SetInfoText()
    {
        Creature.Attack a = creature.attacks[selectedWeapon];
        float cooldownTime = a.cooldown;
        if (a.action.ToLower() != "free action" && a.action.ToLower() != "special")
            cooldownTime = Mathf.Max(a.cooldown, resetFireCooldown);
        if (cooldownTime > 0)
            infoText.color = Color.red;
        else
            infoText.color = Color.white;
        cooldownTime = Mathf.Round(cooldownTime * 10) / 10f;
        infoText.text = (cooldownTime > 0 ? cooldownTime.ToString() : GameManager.instance.getControlMapping("Fire1")) + "\n" + a.name + "\n" + a.action + "\n" + (a.save != null && a.save.Length > 0 ? "DC " + a.num + " " + a.save + " save" : "+" + a.num + " to hit") + (a.damageType != null && a.damageType.Length > 0 ? "\n" + a.damage + " " + a.damageType : "") + (a.description != null && a.description.Length > 0 ? "\n" + a.description : "");
        infoText3.color = infoText.color;
        infoText3.text = infoText.text;

    }

    float resetFireCooldown;

    IEnumerator ResetFire(float timesPerTurn)
    {
        canShoot = false;
        float waitTime = timesPerTurn > 0 ? 6 / (timesPerTurn * GameManager.instance.matchSettings.speedMult) : 0;
        resetFireCooldown = waitTime;
        if (waitTime > 0.1f)
            while (resetFireCooldown > 0)
            {
                yield return new WaitForSeconds(0.1f);
                resetFireCooldown -= 0.1f;
                SetInfoText();
            }
        canShoot = true;
    }

}
