using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Beholder : Creature {

    string shootSound = "lazer-high-pitch";
    string hitSound = "blast";

    public int laserNum = 0;

    public override void syncCreatureInstance(Creature c)
    {
        laserNum = ((Beholder)c).laserNum;
    }


    public override float[] getCreatureDataFloats()
    {
        return base.getCreatureDataFloats();
    }

    public override int[] getCreatureDataInts()
    {
        return new int[]{ laserNum };
        //return base.getCreatureDataInts();
    }

    public override void syncCreatureData(float[] floats, int[] ints)
    {
        laserNum = ints[0];
        base.syncCreatureData(floats, ints);
    }

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
        //Debug.Log("2");
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
