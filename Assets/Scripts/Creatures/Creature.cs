using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Creature : NetworkBehaviour {

    public const string PLAYER_TAG = "Player";

    public GameObject player;

    //public PlayerWeapon weapon;

    public Camera cam;

    public LayerMask mask;

    [System.Serializable]
    public struct Attack
    {
        public string name;
        public string action;
        public int num;
        public string save;
        public int range;
        public int damage;
        public string damageType;
        public string description;
        public float cooldown;
    }

    [System.Serializable]
    public struct Ability
    {
        public string name;
        public string action;
        public string description;
        public int cooldown;
    }

    public Attack[] attacks;
    public Ability[] abilities;

    public virtual float primary()
    {
        return 0;
    }

    public virtual float primary(int selected = 0)
    {
        return primary();
    }

    public virtual void aim(float amount)
    {
        if (cam != null)
            cam.fieldOfView *= amount;
        if (GetComponent<Player>() != null && GetComponent<Player>().cam3 != null)
            GetComponent<Player>().cam3.fieldOfView *= amount;
        //float distOff = 0.2f;
        //if (isLocalPlayer)
        //    GetComponent<Player>().healthText.transform.localPosition = cam.transform.forward * 0.2f * distOff + cam.transform.up * 0.1f * distOff * cam.fieldOfView / 60;
    }

    public virtual float alternative()
    {
        return 0;
    }

    public virtual float alternative(int selected = 0)
    {
        return alternative();
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

    public virtual void walkAnim(float speed, Vector3 dir)
    {
        walkAnim(speed);
    }

    public virtual void fallAnim(float speed, Vector3 dir)
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
