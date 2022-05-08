using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elizabeth : Creature {

    Animator anim;
    bool flapped = false;
    [UnityEngine.Networking.Client]
    void Shoot(float damage, string dmgType, int num, string save, int range)
    {
        //Debug.Log("2");
        //GameObject emitter = player.GetComponent<Player>().getPrimaryEmitter();
        //GameObject emitter = cam.gameObject;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, mask))
        {
            // We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG && player.GetComponent<Player>().isLocalPlayer)
            {
                player.GetComponent<PlayerShoot>().CmdPlayerShot(hit.collider.name, damage, dmgType, num, save);
            }
        }
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override float primary()
    {
        anim.SetBool("isPunching", true);
        anim.Play("Punch");
        StartCoroutine(CancelAnim("isPunching", 0.65f));
        //anim.Play("Punch");
        Shoot(3, "bludgeoning", 3, null, 2);
        return 2;
        //return base.primary();
    }

    public override float ability(bool canShoot, int n)
    {
        /*
        anim.Play("Watch");
        anim.SetBool("isWatching", true);
        if (wCount < 4)
        {
            wCount++;
            StartCoroutine(StopWatching(0.2f));
        }
        return 0;*/
        return base.ability(canShoot, n);
    }

    public override void walkAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        //anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", true);
        if (!anim.GetBool("isPunching") && !anim.GetBool("isPointing"))
        {
            anim.Play("Walk");
            if (!flapped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.25f) < 0.1f)
            {
                //GameManager.instance.sound.PlayAtObject("footstep_harpy", this.gameObject, 0.1f, 0.7f + 0.1f * Random.Range(1, 3) * Mathf.Min(4, speed), 20f);
                flapped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.5f) < 0.1f)
                flapped = false;
            else if (!flapped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.69f) < 0.1f)
            {
                //GameManager.instance.sound.PlayAtObject("footstep_harpy", this.gameObject, 0.1f, 0.7f + 0.1f * Random.Range(1, 3) * Mathf.Min(4, speed), 20f);
                flapped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.1f) < 0.1f)
                flapped = false;
        }
    }

    /*public override void specialAnim(float speed)
    {
        anim.SetBool("isWatching", true);
        if (!anim.GetBool("isPunching"))
        {
            anim.Play("Watch");
        }
    }*/

    public override void idleAnim()
    {
        //idleSound();

        if (!anim.GetBool("isPunching") && !anim.GetBool("isPointing"))
            anim.Play("Idle");
        //anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", false);
    }

    IEnumerator CancelAnim(string a, float t)
    {
        yield return new WaitForSeconds(t);
        anim.SetBool(a, false);
    }

    /*
    int wCount = 0;
    IEnumerator StopWatching(float t)
    {
        anim.SetBool("isWatching", true);
        yield return new WaitForSeconds(t);
        wCount -= 1;
        if (wCount <= 0)
            anim.SetBool("isWatching", false);
    }*/
}
