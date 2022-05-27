using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonsRilae : Creature {

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

    public GameObject facePlate;

    Animator anim;
    bool flapped = false;
    

    [UnityEngine.Networking.Client]
    void Shoot(int num, string save, float damage, string dmgType, float range)
    {
        range = 2.5f + range / 3;
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
            GameManager.instance.otherHitStuff(hit, damage, dmgType, num, save, player);
        }
    }

    Attack[] attacksBackup;
    ParticleSystem[] particles;

    private void Start()
    {
        particles = GetComponent<Character>().particles;
        //watch = new CarlWatch(songs, gameObject);
        anim = GetComponent<Animator>();
        /*int sampleFreq = 44000;
        float frequency = 440;

        float[] samples = new float[44000];
        for (int i = 0; i < samples.Length; i++)
        {
            samples[i] = Mathf.Sin(Mathf.PI * 2 * i * frequency / sampleFreq);
        }
        AudioClip ac = AudioClip.Create("Test", samples.Length, 1, sampleFreq, false);
        ac.SetData(samples, 0);
        GameManager.instance.sound.PlayAtObject(ac, player.gameObject, 0.1f);*/
        attacksBackup = attacks;
    }

    public override float primary(int sel)
    {
        if (elevatorSelecting || !canElevator)
            return base.primary(sel);

        Attack a = attacks[sel];

        int j = 0;
        switch (sel)
        {
            case 0: // Unarmed Strike
                j = Random.Range(1, 4);
                GameManager.instance.sound.PlayAtObject("whoosh" + (j == 1 ? "" : "" + j), this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
                anim.SetBool("isCasting", true);
                anim.Play("Slap");
                StartCoroutine(CancelAnim("isCasting", 0.55f, true));
                Shoot(a.num, a.save, a.damage, a.damageType, a.range);
                return 2;
            case 1: // Pulse Wave
                anim.SetBool("isCasting", true);
                anim.Play("Pulse Wave");
                StartCoroutine(CancelAnim("isCasting", 1.66f, true));
                Shoot(a.num, a.save, a.damage, a.damageType, a.range);
                break;
            case 2: // Gravity Sinkhole
                anim.SetBool("isCasting", true);
                anim.Play("Gravity Sinkhole");
                StartCoroutine(CancelAnim("isCasting", 2, true));
                Shoot(a.num, a.save, a.damage, a.damageType, a.range);
                break;
            case 3: // Gravity Fissure
                anim.SetBool("isCasting", true);
                anim.Play("Gravity Fissure");
                StartCoroutine(CancelAnim("isCasting", 2.79f, true));
                Shoot(a.num, a.save, a.damage, a.damageType, a.range);
                break;
        }
        return 1;
        //return base.primary();
    }

    public override void damage(float amount = 0)
    {
        if (player.GetComponent<Player>().health > 0)
        {
            //int j = Random.Range(0, 14);
            //GameManager.instance.sound.PlayAtObject("CarlHurt" + j, this.gameObject, 0.8f, 1 + 0.1f * Random.Range(-1, 1), 20f);
        } else
        {
            //int j = Random.Range(0, 4);
            //GameManager.instance.sound.PlayAtObject("CarlDeath" + j, this.gameObject, 0.8f, 1 + 0.1f * Random.Range(-1, 1), 20f);
        }
        base.damage(amount);
    }

    public override float ability(bool canShoot, int n)
    {
       
        return 0;
        //return base.ability(canShoot, n);
    }

    public override void walkAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", true);
        if (!anim.GetBool("isCasting"))
        {
            if (anim.GetBool("isMeditating"))
                anim.Play("Spell Prepare");
            else
                anim.Play("Walk");
            
            
        }
        anim.Play("ForceWalk");
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

        if (particles[0].isPlaying)
            for (int i = 0; i < particles.Length; i++)
                if (particles[i].isPlaying)
                    particles[i].Stop();
    }

    public override void specialAnim(float speed)
    {
        anim.SetBool("isWatching", true);
        if (!anim.GetBool("isPunching"))
        {
            anim.Play("Watch");
        }
    }

    public override void idleAnim()
    {
        //idleSound();

        if (!anim.GetBool("isCasting"))
        {
            if (anim.GetBool("isMeditating"))
                anim.Play("Spell Prepare");
            else
                anim.Play("Idle");
        }
        anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", false);
        anim.Play("Null");
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
        if (!anim.GetBool("isCasting") && !anim.GetBool("isMeditating"))
        {
            anim.Play("Fly");
            
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
        } else
        {
            if (!anim.GetBool("isCasting") && anim.GetBool("isMeditating"))
                    anim.Play("Spell Prepare");

                for (int i = 0; i < 2; i++)
                    if (!particles[i].isPlaying)
                        particles[i].Play();
                for (int i = 2; i < 4; i++)
                    if (particles[i].isPlaying)
                        particles[i].Stop();
        }
        anim.Play("ForceFly");
    }

    IEnumerator CancelAnim(string a, float t, bool meditate = false)
    {
        yield return new WaitForSeconds(t);
        anim.SetBool(a, false);
        if (meditate)
        {
            anim.SetBool("isMeditating", true);
            StartCoroutine(CancelAnim("isMeditating", 0.83f));
        }
    }

    IEnumerator IdleSound()
    {
        yield return new WaitForSeconds(Random.Range(2f, 8f));

        //int j = Random.Range(0, 3);
        //GameManager.instance.sound.PlayAtObject("CarlBreathe" + j, this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);

        StartCoroutine(IdleSound());
    }

    int td = 0;
    int sd = 0;

    private void idleSound()
    {
        sd++;
        if (sd > 400)
        {
            //int j = Random.Range(0, 3);
            //GameManager.instance.sound.PlayAtObject("CarlBreathe" + j, this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
            sd = Random.Range(-50, 50);
        }
    }


}
