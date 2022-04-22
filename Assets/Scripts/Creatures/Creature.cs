﻿using System.Collections;
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

    public virtual void aim()
    {

    }

    public virtual float alternative()
    {
        return 0;
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

    public virtual void damage(float amount = 0f)
    {

    }
}
