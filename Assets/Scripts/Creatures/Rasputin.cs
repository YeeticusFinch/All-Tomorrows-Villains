using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rasputin : Creature {

    Animator anim;
    bool flapped = false;
    bool flying = false;

    public override void syncCreatureInstance(Creature c)
    {
        
    }

    public override float[] getCreatureDataFloats()
    {
        return base.getCreatureDataFloats();
    }
    
    public override int[] getCreatureDataInts()
    {
        return base.getCreatureDataInts();
    }

    public override void syncCreatureData(float[] floats, int[] ints)
    {
        base.syncCreatureData(floats, ints);
    }

    [UnityEngine.Networking.Client]
    void Shoot(float damage, string dmgType)
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
                player.GetComponent<PlayerShoot>().CmdPlayerShot(hit.collider.name, damage, dmgType, 7, null);
            }
        }
    }

    ParticleSystem[] particles;

    private void Start()
    {
        particles = GetComponent<Character>().particles;
        anim = GetComponent<Animator>();
    }

    public override float primary()
    {
        anim.SetBool("isPunching", true);
        if (flying)
            anim.Play("Fly Punch");
        else
            anim.Play("Punch");
        StartCoroutine(CancelAnim("isPunching", 0.5f));
        //anim.Play("Punch");
        Shoot(7, "bludgeoning");
        return 3;
        //return base.primary();
    }

    public override float ability(bool canShoot, int n)
    {
        anim.Play("Kazachok");
        anim.SetBool("isKazachoking", true);
        if (kCount < 4)
        {
            kCount++;
            StartCoroutine(StopKazachok(0.3f));
        }
        return 0;
        //return base.ability(canShoot, n);
    }

    public override void walkAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", true);
        if (!anim.GetBool("isPunching") && !anim.GetBool("isShooting") && !anim.GetBool("isKazachoking"))
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
        if (particles[0].isPlaying)
            for (int i = 0; i < particles.Length; i++)
                if (particles[i].isPlaying)
                    particles[i].Stop();
    }

    public override void specialAnim(float speed)
    {
        anim.SetBool("isKazachoking", true);
        if (!anim.GetBool("isPunching") && !anim.GetBool("isShooting")) {
            anim.Play("Kazachok");
            if (particles[0].isPlaying)
                for (int i = 0; i < particles.Length; i++)
                    if (particles[i].isPlaying)
                        particles[i].Stop();
        }
    }

    public override void idleAnim()
    {
        //idleSound();

        if (!anim.GetBool("isPunching") && !anim.GetBool("isShooting") && !anim.GetBool("isKazachoking"))
            anim.Play("Idle");
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", false);
        if (particles[0].isPlaying)
            for (int i = 0; i < particles.Length; i++)
                if (particles[i].isPlaying)
                    particles[i].Stop();
    }
    public override void flyAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        anim.SetBool("isWalking", false);
        anim.SetBool("isFlying", true);
        if (!anim.GetBool("isPunching") && !anim.GetBool("isShooting") && !anim.GetBool("isKazachoking"))
        {
            anim.Play("Fly");
            if (!particles[0].isPlaying)
                for (int i = 0; i < particles.Length; i++)
                    if (!particles[i].isPlaying)
                        particles[i].Play();
            //if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime
            /*if (GameManager.instance != null && !flapped && Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.3f) < 0.1f)
            {
                //GameManager.instance.sound.PlayAtObject("harpy_flap", this.gameObject, 1.3f, 0.3f + 0.1f * Random.Range(1, 4) * Mathf.Min(4, speed), 20f);
                flapped = true;
            }
            else if (Mathf.Abs(anim.GetCurrentAnimatorStateInfo(0).normalizedTime - (int)anim.GetCurrentAnimatorStateInfo(0).normalizedTime - 0.1f) < 0.1f)
                flapped = false;*/
            //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
    }

    IEnumerator CancelAnim(string a, float t)
    {
        yield return new WaitForSeconds(t);
        anim.SetBool(a, false);
    }

    int kCount = 0;
    IEnumerator StopKazachok(float t)
    {
        anim.SetBool("isKazachoking", true);
        yield return new WaitForSeconds(t);
        kCount -= 1;
        if (kCount <= 0)
            anim.SetBool("isKazachoking", false);
    }

}
