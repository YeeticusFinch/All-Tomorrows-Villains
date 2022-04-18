
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    //[SerializeField]
    //public AudioSource shoot;

    private const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField]
    public Camera cam;

    [SerializeField]
    private LayerMask mask;

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
    }

    public void Init(Character chara)
    {
        creature = chara.creature;
        creature.mask = mask;
        creature.cam = cam;
        creature.weapon = weapon;
        creature.player = player;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (PauseGame.IsOn)
                return;

            if (Input.GetButton("Fire1") && canShoot)
            {
                canShoot = false;
                CmdPrimary();
                //Shoot();
            }
        }
    }

    [Command]
    private void CmdPrimary()
    {
        RpcPrimary();
    }

    [ClientRpc]
    private void RpcPrimary()
    {
        float x = creature.primary();
        if (isLocalPlayer)
            StartCoroutine(ResetFire(x));
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
    public void CmdPlayerShot(string playerId, float damage)
    {
        //Debug.Log(playerId + " has been shot.");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
    }

    

    IEnumerator ResetFire(float timesPerTurn)
    {
        canShoot = false;
        yield return new WaitForSeconds(6/(timesPerTurn * GameManager.instance.matchSettings.speedMult));
        canShoot = true;
    }

}
