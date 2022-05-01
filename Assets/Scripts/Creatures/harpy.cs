using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class harpy : Creature
{
    Animator anim;
    bool swing = false;
    bool shock = false;

    int sd = 0;
    int td = 0;

    public float dmgWeapon = 3;
    public string dmgWeaponType = "bludgeoning";
    public int weaponToHit = 3;
    public float dmgClaw = 6;
    public string dmgClawType = "slashing";
    public int clawToHit = 3;

    public bool shockingGrasp = false;
    public float dmgShock;
    public int shockToHit;

    [Client]
    void Shoot(int toHit, float damage, string dmgType)
    {
        //Debug.Log("2");
        //GameObject emitter = player.GetComponent<Player>().getPrimaryEmitter();
        //GameObject emitter = cam.gameObject;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2, mask))
        {
            // We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG && player.GetComponent<Player>().isLocalPlayer)
            {
                player.GetComponent<PlayerShoot>().CmdPlayerShot(hit.collider.name, damage, dmgType, toHit, null);
            }
        }
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(IdleSound());
    }

    public override float primary()
    {
        //Shoot();
        if (swing)
        {
            int j = Random.Range(1, 4);
            GameManager.instance.sound.PlayAtObject("whoosh" + (j == 1 ? "" : ""+j), this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
            anim.Play("Swing");
            anim.SetBool("isSwinging", true);
            StartCoroutine(CancelAnim("isSwinging",0.5f));
            Shoot(weaponToHit, dmgWeapon, dmgWeaponType);
            swing = false;
            if (shockingGrasp)
                shock = true;
        } else if (shock)
        {
            //GameManager.instance.sound.PlayAtObject("blast", this.gameObject, 0.8f, 1 + 0.1f * Random.Range(-4, 4), 80f);
            Effects.instance.Sparky(player.GetComponent<Player>().getPrimaryEmitter().transform.position - player.GetComponent<Player>().getPrimaryEmitter().transform.right*0.8f, cam.transform.position + cam.transform.forward * 2, "lazer-high-pitch", "blast", new Color(1.2f, 1.8f, 3f));
            anim.Play("Swing");
            anim.SetBool("isSwinging", true);
            StartCoroutine(CancelAnim("isSwinging", 0.5f));
            Shoot(shockToHit, dmgShock, "lightning");
            shock = false;
        } else
        {
            //int j = Random.Range(1, 3);
            GameManager.instance.sound.PlayAtObject("sword_swing", this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-1, 1), 20f);
            anim.Play("Claw");
            anim.SetBool("isClawing", true);
            StartCoroutine(CancelAnim("isClawing", 0.5f));
            Shoot(clawToHit, dmgClaw, dmgClawType);
            swing = true;
        }
        //swing = !swing;
        return shockingGrasp ? 3 : 2;
        //return base.primary();
    }

    bool flapped = false;

    public override void walkAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", true);
        if (!anim.GetBool("isSwinging") && !anim.GetBool("isClawing"))
        {
            anim.Play("Walk");
            if (!flapped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.25f) < 0.1f)
            {
                GameManager.instance.sound.PlayAtObject("footstep_harpy", this.gameObject, 0.1f, 0.7f + 0.1f * Random.Range(1, 3) * Mathf.Min(4, speed), 20f);
                flapped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.5f) < 0.1f)
                flapped = false;
            else if (!flapped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.69f) < 0.1f)
            {
                GameManager.instance.sound.PlayAtObject("footstep_harpy", this.gameObject, 0.1f, 0.7f + 0.1f * Random.Range(1, 3) * Mathf.Min(4, speed), 20f);
                flapped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.1f) < 0.1f)
                flapped = false;
        }
        if (speed > 1.5f)
        {
            tiredSound(speed);
        }
    }

    public override void idleAnim()
    {
        //idleSound();
            
        if (!anim.GetBool("isSwinging") && !anim.GetBool("isClawing"))
            anim.Play("Idle");
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", false);
    }
    public override void flyAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        anim.SetBool("isWalking", false);
        anim.SetBool("isFlying", true);
        if (!anim.GetBool("isSwinging") && !anim.GetBool("isClawing"))
        {
            anim.Play("Fly");
            //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime
            if (GameManager.instance != null && !flapped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.3f) < 0.1f)
            {
                GameManager.instance.sound.PlayAtObject("harpy_flap", this.gameObject, 1.3f, 0.3f + 0.1f * Random.Range(1, 4) * Mathf.Min(4, speed), 20f);
                flapped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.1f) < 0.1f)
                flapped = false;
            //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        if (speed > 1.3f)
        {
            tiredSound(speed);
        }
    }

    public override void damage(float amount = 0)
    {
        int j = Random.Range(1, 5);
        GameManager.instance.sound.PlayAtObject("harpy_hurt" + (j == 1 ? "" : "" + j), this.gameObject, 0.6f, 1 + 0.1f * Random.Range(-1, 1), 20f);
        base.damage(amount);
    }

    IEnumerator CancelAnim(string a, float t)
    {
        yield return new WaitForSeconds(t);
        anim.SetBool(a, false);
    }

    IEnumerator IdleSound()
    {
        yield return new WaitForSeconds(Random.Range(2f, 8f));
        if (Random.Range(0, 2) == 0)
        {
            int j = Random.Range(1, 3);
            GameManager.instance.sound.PlayAtObject("harpy_idle" + (j == 1 ? "" : "" + j), this.gameObject, 0.3f, 1 + 0.03f * Random.Range(-4, 4), 20f);
        }
        else
        {
            int j = Random.Range(1, 7);
            GameManager.instance.sound.PlayAtObject("harpy_scream" + (j == 1 ? "" : "" + j), this.gameObject, 0.3f, 1 + 0.03f * Random.Range(-4, 4), 20f);
        }
        
        StartCoroutine(IdleSound());
    }
    
    private void tiredSound(float x)
    {
        //Debug.Log("Speed = " + x);
        td+=(int)x;
        if (td > 500)
        {
            int j = Random.Range(1, 5);
            GameManager.instance.sound.PlayAtObject("harpy_breathe" + (j == 1 ? "" : "" + j), this.gameObject, 0.5f, 1 + 0.1f * Random.Range(-2, 2), 20f);
            td = Random.Range(-20, 20);
        }
    }

    private void idleSound()
    {
        sd++;
        if (sd > 400)
        {
            if (Random.Range(0, 2) == 0)
            {
                int j = Random.Range(1, 3);
                GameManager.instance.sound.PlayAtObject("harpy_idle" + (j == 1 ? "" : "" + j), this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
            }
            else
            {
                int j = Random.Range(1, 7);
                GameManager.instance.sound.PlayAtObject("harpy_scream" + (j == 1 ? "" : "" + j), this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
            }
            sd = Random.Range(-50, 50);
        }
    }

}
