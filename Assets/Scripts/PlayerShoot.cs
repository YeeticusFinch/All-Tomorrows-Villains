
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour {

    //[SerializeField]
    //public AudioSource shoot;

    string shootSound = "lazer-high-pitch";
    string hitSound = "blast";

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

    [SerializeField]
    private Material glow;

    private bool canShoot = true;

    private void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (PauseGame.IsOn)
            return;

        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            StartCoroutine(ResetFire());
            Shoot();
        }
    }

    [Client]
    private void Shoot()
    {
        GameObject emitter = GetComponent<Player>().getPrimaryEmitter();
        RaycastHit hit;
        CmdSparky(emitter.transform.position, emitter.transform.position + 0.03f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), false, false);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            CmdSparky(emitter.transform.position, hit.point, true, true);
            CmdSparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), false, false);
            CmdSparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), false, false);
            CmdSparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), false, false);
            // We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, weapon.damage);
            }
        } else
        {
            CmdSparky(emitter.transform.position, emitter.transform.position + cam.transform.forward * weapon.range, true, false);
        }
    }

    [Command]
    public void CmdPlayerShot(string playerId, float damage)
    {
        Debug.Log(playerId + " has been shot.");

        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
    }

    [Command]
    void CmdSparky(Vector3 pos1, Vector3 pos2, bool sound, bool hitSound)
    {
        RpcSparky(pos1, pos2, sound, hitSound);
    }

    [ClientRpc]
    void RpcSparky(Vector3 pos1, Vector3 pos2, bool sound, bool hitSound)
    {
        //shoot.Play();
        if (sound)
            GameManager.instance.sound.playAt(shootSound, pos1, 0.5f, 0.1f*Random.Range(7, 13), 100f);
        if (hitSound)
            GameManager.instance.sound.playAt(this.hitSound, pos2, 1f, 0.1f * Random.Range(5, 15), 150f);
        StartCoroutine(Sparky(pos1, pos2));
    }

    IEnumerator ResetFire()
    {
        canShoot = false;
        yield return new WaitForSeconds(6/(GetComponent<Player>().chara.attackSpeed * GameManager.instance.matchSettings.speedMult));
        canShoot = true;
    }

    IEnumerator Sparky(Vector3 pos1, Vector3 pos2)
    {
        int c = Random.Range(5, Mathf.Max(7, (int)(2*Mathf.Abs((pos2-pos1).magnitude))));

        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.material = glow;
        lineRenderer.material.color = new Color(Random.Range(0, 100)/7f, Random.Range(0, 100)/7f, Random.Range(0, 100)/7f);
        
        switch (Random.Range(0, 8))
        {
            case 0:
            case 1:
            case 2:
                lineRenderer.startColor = Color.magenta;
                break;
            case 3:
                lineRenderer.startColor = Color.yellow;
                break;
            case 4:
            case 5:
                lineRenderer.startColor = Color.cyan;
                break;
            case 6:
            case 7:
                lineRenderer.startColor = Color.white;
                break;
        }
        switch (Random.Range(0, 8))
        {
            case 0:
            case 1:
            case 2:
                lineRenderer.endColor = Color.magenta;
                break;
            case 3:
                lineRenderer.endColor = Color.yellow;
                break;
            case 4:
            case 5:
                lineRenderer.endColor = Color.cyan;
                break;
            case 6:
            case 7:
                lineRenderer.endColor = Color.white;
                break;
        }

        lineRenderer.SetColors(lineRenderer.startColor, lineRenderer.endColor);
        //lineRenderer.colorGradient = new Gradient(lineRenderer.startColor, lineRenderer.endColor);

        lineRenderer.startWidth = 0.0006f * Random.Range(1, 200);
        lineRenderer.positionCount = c;
        lineRenderer.useWorldSpace = true;

        float stepX = (pos1.x - pos2.x) / c;
        float stepY = (pos1.y - pos2.y) / c;
        float stepZ = (pos1.z - pos2.z) / c;

        lineRenderer.SetPosition(0, new Vector3(pos1.x + Random.Range(-5, 5) / 200f, pos1.y + Random.Range(-5, 5) / 200f, pos1.z + Random.Range(-5, 5) / 200f));
        for (int i = 1; i < c - 1; i++)
            lineRenderer.SetPosition(i, new Vector3(pos1.x - i * stepX + Random.Range(-100, 100) / 200f, pos1.y - i * stepY + Random.Range(-100, 100) / 200f, pos1.z - i * stepZ + Random.Range(-100, 100) / 200f));
        lineRenderer.SetPosition(c - 1, new Vector3(pos2.x, pos2.y, pos2.z));

        yield return new WaitForSeconds(0.045f);

        GameObject.Destroy(lineRenderer.gameObject);
        //yield break;
    }

}
