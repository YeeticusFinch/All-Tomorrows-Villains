using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tolivunt : Creature
{
    Animator anim;

    bool stepped = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        //StartCoroutine(IdleSound());
    }

    public override void walkAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        //anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", true);
        if (/*!anim.GetBool("isSwinging") && !anim.GetBool("isClawing")*/true)
        {
            anim.Play("Walk");
            if (!stepped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.25f) < 0.1f)
            {
                GameManager.instance.sound.PlayAtObject("footstep_harpy", this.gameObject, 0.1f, 0.4f + 0.1f * Random.Range(1, 3) * Mathf.Min(4, speed), 20f);
                stepped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.5f) < 0.1f)
                stepped = false;
            else if (!stepped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.69f) < 0.1f)
            {
                GameManager.instance.sound.PlayAtObject("footstep_harpy", this.gameObject, 0.1f, 0.4f + 0.1f * Random.Range(1, 3) * Mathf.Min(4, speed), 20f);
                stepped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.1f) < 0.1f)
                stepped = false;
        }
        //if (speed > 1.5f)
        //{
        //    tiredSound(speed);
        //}
    }

    public override void idleAnim()
    {
        //idleSound();

        if (!anim.GetBool("isSwinging") && !anim.GetBool("isClawing"))
            anim.Play("Idle");
        //anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", false);
    }
}
