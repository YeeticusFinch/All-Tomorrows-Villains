using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Beholder : Creature {

    string shootSound = "lazer-high-pitch";
    string hitSound = "blast";

    public override float primary()
    {
        Shoot();
        return 3f;
        return base.primary();
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
        GameObject emitter = player.GetComponent<Player>().getPrimaryEmitter();
        RaycastHit hit;
        Effects.instance.Sparky(emitter.transform.position, emitter.transform.position + 0.03f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask))
        {
            Effects.instance.Sparky(emitter.transform.position, hit.point, shootSound, hitSound);
            Effects.instance.Sparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            Effects.instance.Sparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            Effects.instance.Sparky(hit.point, hit.point + 0.32f * (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))), null, null);
            // We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG)
            {
                player.GetComponent<PlayerShoot>().CmdPlayerShot(hit.collider.name, weapon.damage);
            }
        }
        else
        {
            Effects.instance.Sparky(emitter.transform.position, emitter.transform.position + cam.transform.forward * weapon.range, shootSound, null);
        }
    }
}
