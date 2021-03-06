using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestSphere : Creature {

    string shootSound = "lazer-high-pitch";
    string hitSound = "blast";


    public override float primary()
    {
        Shoot();
        return 2f;
        //return base.primary();
    }
    /*
    public override void aim()
    {
        base.aim();
    }*/

    public override float alternative()
    {
        return base.alternative();
    }

    [Client]
    void Shoot()
    {

        //Debug.Log("6");
        GameObject emitter = player.GetComponent<Player>().getPrimaryEmitter();
        RaycastHit hit;
        Effects.instance.Sparky(emitter.transform.position, emitter.transform.position + 0.03f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 200, mask))
        {
            Effects.instance.Sparky(emitter.transform.position, hit.point, shootSound, hitSound);
            Effects.instance.Sparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            Effects.instance.Sparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            Effects.instance.Sparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            // We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG && player.GetComponent<Player>().isLocalPlayer)
            {
                player.GetComponent<PlayerShoot>().CmdPlayerShot(hit.collider.name, 10, "force", 20, null);
            }
        }
        else
        {
            Effects.instance.Sparky(emitter.transform.position, emitter.transform.position + cam.transform.forward * 200, shootSound, null);
        }
    }

}
