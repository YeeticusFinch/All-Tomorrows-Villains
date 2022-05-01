using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Creature : NetworkBehaviour {

    public const string PLAYER_TAG = "Player";

    public GameObject player;

    public PlayerWeapon weapon;

    public Camera cam;

    public LayerMask mask;

    public virtual float primary()
    {
        return 0;
    }

    public virtual void aim(float amount)
    {
        cam.fieldOfView *= amount;
        //float distOff = 0.2f;
        //if (isLocalPlayer)
        //    GetComponent<Player>().healthText.transform.localPosition = cam.transform.forward * 0.2f * distOff + cam.transform.up * 0.1f * distOff * cam.fieldOfView / 60;
    }

    public virtual float alternative()
    {
        return 0;
    }

    public virtual float ability(bool canShoot, int n)
    {
        return -1;
    }

    public virtual float ability2(bool canShoot)
    {
        return -1;
    }

    public virtual void walkAnim(float speed)
    {

    }

    public virtual void flyAnim(float speed)
    {

    }

    public virtual void idleAnim()
    {

    }

    public virtual void specialAnim(float speed)
    {

    }

    public virtual void damage(float amount = 0f)
    {

    }
}
