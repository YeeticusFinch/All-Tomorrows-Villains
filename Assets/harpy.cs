using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class harpy : Creature
{
    Animator anim;
    bool swing = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public override float primary()
    {
        //Shoot();
        if (swing)
        {
            anim.Play("Swing");
            anim.SetBool("isSwinging", true);
            StartCoroutine(CancelAnim("isSwinging",0.5f));
        }
        else
        {
            anim.Play("Claw");
            anim.SetBool("isClawing", true);
            StartCoroutine(CancelAnim("isClawing", 0.5f));
        }
        swing = !swing;
        return 2;
        //return base.primary();
    }

    public override void walkAnim(float speed)
    {
        anim.SetFloat("speed", speed);
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", true);
        if (!anim.GetBool("isSwinging") && !anim.GetBool("isClawing"))
            anim.Play("Walk");
    }

    public override void idleAnim()
    {
        if (!anim.GetBool("isSwinging") && !anim.GetBool("isClawing"))
            anim.Play("Idle");
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", false);
    }

    public override void flyAnim(float speed)
    {
        anim.SetFloat("speed", speed);
        anim.SetBool("isWalking", false);
        anim.SetBool("isFlying", true);
        if (!anim.GetBool("isSwinging") && !anim.GetBool("isClawing"))
            anim.Play("Fly");
    }

    IEnumerator CancelAnim(string a, float t)
    {
        yield return new WaitForSeconds(t);
        anim.SetBool(a, false);
    }
}
