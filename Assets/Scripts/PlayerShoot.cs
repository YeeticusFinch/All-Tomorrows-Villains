
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    //[SerializeField]
    //public AudioSource shoot;

    private const string PLAYER_TAG = "Player";

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
        }
    }

    public void Init(Character chara)
    {
        creature = chara.creature;
        creature.mask = mask;
        creature.cam = cam;
        //creature.weapon = weapon;
        creature.player = player;
        if (GetComponent<PlayerController>())
            GetComponent<PlayerController>().creature = creature;
    }
    int camMode = 2;
    private void Update()
    {
        if (isLocalPlayer)
        {
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
                CmdPrimary();
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
    private void CmdPrimary()
    {
        //Debug.Log("3");
        RpcPrimary();
    }

    [ClientRpc]
    private void RpcPrimary()
    {
        //Debug.Log("4");
        float x = creature.primary();
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

    

    IEnumerator ResetFire(float timesPerTurn)
    {
        canShoot = false;
        yield return new WaitForSeconds(6/(timesPerTurn * GameManager.instance.matchSettings.speedMult));
        canShoot = true;
    }

}
