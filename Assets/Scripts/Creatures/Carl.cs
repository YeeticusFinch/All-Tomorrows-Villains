using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carl : Creature {

    public override void syncCreatureInstance(Creature c)
    {
        songSelecting = ((Carl)c).songSelecting;
    }

    public override float[] getCreatureDataFloats()
    {
        return base.getCreatureDataFloats();
    }

    public override int[] getCreatureDataInts()
    {
        return new int[] { songSelecting ? 1 : 0 };
        //return base.getCreatureDataInts();
    }

    public override void syncCreatureData(float[] floats, int[] ints)
    {
        songSelecting = ints[0] == 1 ? true : false;
        if (songSelecting)
        {
            attacks = new Attack[songs.Length + 1];
            attacks[0] = new Attack("Exit Song Select", "bonus action", 0, null, 0, 0, null, "Exits song selector and returns to your loadout");
            for (int i = 1; i < songs.Length + 1; i++)
            {
                Debug.Log(i + " < " + attacks.Length + " < " + watch.songNames.Length + " < " + watch.fileNames.Length);
                attacks[i] = new Attack(watch.songNames[i - 1], "bonus action", 0, null, 0, 0, null, watch.fileNames[i - 1]);
            }
        }
        base.syncCreatureData(floats, ints);
    }

    Animator anim;
    bool flapped = false;

    public TextAsset[] songs;

    private CarlWatch watch;

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

    private void Start()
    {
        if (player != null)
        {
            watch = player.AddComponent<CarlWatch>();
            watch.startShit(songs, player.GetComponent<Player>().getSecondaryEmitter());
            int j = Random.Range(0, 5);
            GameManager.instance.sound.PlayAtObject("CarlSpawn" + j, this.gameObject, 0.6f, 1 + 0.02f * Random.Range(-4, 4), 20f);
            StartCoroutine(IdleSound());
        }
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

    public bool songSelecting = false;

    public override float primary(int sel)
    {
        if (elevatorSelecting || !canElevator)
            return base.primary(sel);
        Attack a = attacks[sel];
        if (songSelecting)
        {
            if (sel == 0)
            {
                songSelecting = false;
                attacks = attacksBackup;
            } else
            {
                watch.playSong(sel - 1);
            }
            return 5;
        }
        if (sel == 0) { // Unarmed strike
            int j = Random.Range(1, 4);
            GameManager.instance.sound.PlayAtObject("whoosh" + (j == 1 ? "" : "" + j), this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
            j = Random.Range(0, 14);
            GameManager.instance.sound.PlayAtObject("CarlAttack" + j, this.gameObject, 0.6f, 1 + 0.02f * Random.Range(-4, 4), 20f);
            anim.SetBool("isPunching", true);
            anim.Play("Punch");
            StartCoroutine(CancelAnim("isPunching", 0.55f));
            //anim.Play("Punch");
            Shoot(a.num, a.save, a.damage, a.damageType, a.range);
            //watch.playSong(Random.Range(0, songs.Length));
            //watch.playSong(101);
            return 2;
        } else if (sel == 1) // Random Watch Song
        {
            watch.playSong(Random.Range(0, songs.Length));
            return 5;
        } else if (sel == 2) // Song Selector
        {
            songSelecting = true;
            attacks = new Attack[songs.Length + 1];
            attacks[0] = new Attack("Exit Song Select", "bonus action", 0, null, 0, 0, null, "Exits song selector and returns to your loadout");
            for (int i = 1; i < songs.Length+1; i++)
            {
                Debug.Log(i + " < " + attacks.Length + " < " + watch.songNames.Length + " < " + watch.fileNames.Length);
                attacks[i] = new Attack(watch.songNames[i-1], "bonus action", 0, null, 0, 0, null, watch.fileNames[i-1]);
            }
            return 5;
        } else if (sel == 3) // Coping Saber
        {
            int j = Random.Range(0, 14);
            GameManager.instance.sound.PlayAtObject("CarlAttack" + j, this.gameObject, 0.6f, 1 + 0.02f * Random.Range(-4, 4), 20f);
            Shoot(a.num, a.save, a.damage, a.damageType, a.range);
            return 2;
        } else if (sel == 4) // Taser
        {
            GameManager.instance.sound.PlayAtObject("blast", this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
            int j = Random.Range(0, 14);
            GameManager.instance.sound.PlayAtObject("CarlAttack" + j, this.gameObject, 0.6f, 1 + 0.02f * Random.Range(-4, 4), 20f);
            Shoot(a.num, a.save, a.damage, a.damageType, a.range);
            return 2;
        }
        return base.primary();
    }

    public override void damage(float amount = 0)
    {
        if (player.GetComponent<Player>().health > 0)
        {
            int j = Random.Range(0, 14);
            GameManager.instance.sound.PlayAtObject("CarlHurt" + j, this.gameObject, 0.8f, 1 + 0.1f * Random.Range(-1, 1), 20f);
        } else
        {
            int j = Random.Range(0, 4);
            GameManager.instance.sound.PlayAtObject("CarlDeath" + j, this.gameObject, 0.8f, 1 + 0.1f * Random.Range(-1, 1), 20f);
        }
        base.damage(amount);
    }

    public override float ability(bool canShoot, int n)
    {
        anim.Play("Watch");
        anim.SetBool("isWatching", true);
        if (wCount < 4)
        {
            wCount++;
            StartCoroutine(StopWatching(0.2f));
        }
        return 0;
        //return base.ability(canShoot, n);
    }

    public override void walkAnim(float speed)
    {
        //idleSound();
        anim.SetFloat("speed", speed);
        //anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", true);
        if (!anim.GetBool("isPunching") && !anim.GetBool("isWatching"))
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

            if (speed > 1.5f)
            {
                tiredSound(speed);
            }
        }
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

        if (!anim.GetBool("isPunching") && !anim.GetBool("isWatching"))
            anim.Play("Idle");
        //anim.SetBool("isFlying", false);
        anim.SetBool("isWalking", false);
    }

    IEnumerator CancelAnim(string a, float t)
    {
        yield return new WaitForSeconds(t);
        anim.SetBool(a, false);
    }

    int wCount = 0;
    IEnumerator StopWatching(float t)
    {
        anim.SetBool("isWatching", true);
        yield return new WaitForSeconds(t);
        wCount -= 1;
        if (wCount <= 0)
            anim.SetBool("isWatching", false);
    }

    IEnumerator IdleSound()
    {
        yield return new WaitForSeconds(Random.Range(2f, 8f));

        int j = Random.Range(0, 3);
        GameManager.instance.sound.PlayAtObject("CarlBreathe" + j, this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);

        StartCoroutine(IdleSound());
    }

    int td = 0;
    int sd = 0;

    private void tiredSound(float x)
    {
        //Debug.Log("Speed = " + x);
        td += (int)x;
        if (td > 500)
        {
            int j = Random.Range(0, 4);
            GameManager.instance.sound.PlayAtObject("CarlTired" + j, this.gameObject, 0.5f, 1 + 0.1f * Random.Range(-2, 2), 20f);
            td = Random.Range(-20, 20);
        }
    }

    private void idleSound()
    {
        sd++;
        if (sd > 400)
        {
            int j = Random.Range(0, 3);
            GameManager.instance.sound.PlayAtObject("CarlBreathe" + j, this.gameObject, 0.3f, 1 + 0.1f * Random.Range(-4, 4), 20f);
            sd = Random.Range(-50, 50);
        }
    }


}
