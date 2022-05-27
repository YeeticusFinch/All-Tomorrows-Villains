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

    public bool elevatorSelecting = false;
    protected bool canElevator = true;

    public int currentFloor = 0;

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

        public Attack(string name, string action, int num, string save, int range, int damage, string damageType, string description, float cooldown = 0) : this()
        {
            this.name = name;
            this.action = action;
            this.num = num;
            this.save = save;
            this.range = range;
            this.damage = damage;
            this.damageType = damageType;
            this.description = description;
            this.cooldown = cooldown;
        }
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


    private Attack[] OgAttacks;
    private int OgSel = 0;

    public virtual Creature getCreatureInstance()
    {
        return this;
    }

    public virtual float[] getCreatureDataFloats()
    {
        return null;
    }

    public virtual int[] getCreatureDataInts()
    {
        return null;
    }

    public virtual void syncCreatureData(float[] floats, int[] ints)
    {

    }

    public virtual void syncCreatureInstance(Creature c)
    {

    }

    /*
    public virtual void syncShit(float[] floats, int[] ints, bool[] bools, Vector3[] vectors)
    {

    }*/

    public virtual float primary()
    {
        return 0;
    }

    public virtual float primary(int selected = 0)
    {
        Debug.Log("Can Elevator = " + canElevator);
        if (elevatorSelecting)
        {
            if (selected > 1)
            {
                Vector3 pos0 = player.GetComponent<Player>().cam.transform.position;
                bool on = Elevator.floorElevatorMap[selected - 2].GetComponent<Elevator>().on;
                if (on)
                    player.transform.position = Elevator.floorElevatorMap[selected - 2].transform.position;
                else
                {
                    player.transform.position = Elevator.floorElevatorMap[Random.Range(0, 10)].transform.position;
                    player.GetComponent<Player>().GetPlayerShoot().CmdPlayerShot(player.name, 13.5f, "lightning", 12, "con");
                    StartCoroutine(ElevatorMalfunction(pos0, player.GetComponent<Player>().cam.transform.position));
                }
                Effects.instance.Sparky(pos0, player.GetComponent<Player>().cam.transform.position, "blast", "blast", new Color(1.2f, 1.8f, 3) * 5);
                for (int i = 0; i < 5; i++)
                {
                    Effects.instance.Sparky(player.GetComponent<Player>().cam.transform.position, player.GetComponent<Player>().cam.transform.position + new Vector3(0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10)), "blast", "blast", new Color(1.2f, 1.8f, 3) * 5);
                    Effects.instance.Sparky(pos0, pos0 + new Vector3(0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10)), "blast", "blast", new Color(1.2f, 1.8f, 3) * 5);
                }
            }
            else if (selected == 1)
                Elevator.floorElevatorMap[currentFloor].GetComponent<Elevator>().TOggleOn();
            attacks = OgAttacks;
            elevatorSelecting = false;
            player.GetComponent<Player>().GetPlayerShoot().selectedWeapon = OgSel;
            player.GetComponent<Player>().GetPlayerShoot().skipCooldown = true;
            player.GetComponent<Player>().GetPlayerShoot().SetInfoText();
            StartCoroutine(CanElevatorReseet());
            return 0;
        }
        else if (!canElevator)
            return 0;
        return primary();
    }

    IEnumerator ElevatorMalfunction(Vector3 pos0, Vector3 pos1)
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.05f);
            Effects.instance.Sparky(pos0, pos1, "blast", "blast", new Color(1.2f, 1.8f, 3) * 5);
            Effects.instance.Sparky(pos1, pos1 + new Vector3(0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10)), "blast", "footstep_harpy", new Color(3f, 1.8f, 1.2f) * 8);
            Effects.instance.Sparky(pos0, pos0 + new Vector3(0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10), 0.1f * Random.Range(-10, 10)), "blast", "footstep_harpy", new Color(3f, 1.8f, 1.2f) * 8);
        }
    }

    public virtual void aim(float amount)
    {
        if (cam != null)
            cam.fieldOfView *= amount;
        if (player.GetComponent<Player>() != null && player.GetComponent<Player>().cam3 != null)
            player.GetComponent<Player>().cam3.fieldOfView *= amount;
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

    public virtual void elevatorSelect(int floorNum)
    {
        currentFloor = floorNum;
        if (!canElevator)
            return;
        OgAttacks = attacks;
        elevatorSelecting = true;
        if (Elevator.floorElevatorMap[floorNum].GetComponent<Elevator>().on)
            attacks = new Attack[12];
        else
            attacks = new Attack[2];
        attacks[0] = new Attack("You are on floor " + (floorNum+1), "free action", 0, null, 0, 0, null, "Click to cancel Elevator Select");
        attacks[1] = new Attack("Enable/Disable elevator " + (floorNum + 1), "free action", 0, null, 0, 0, null, "Click to enable/disable this elevator stop");
        OgSel = player.GetComponent<Player>().GetPlayerShoot().selectedWeapon;
        player.GetComponent<Player>().GetPlayerShoot().selectedWeapon = 0;
        player.GetComponent<Player>().GetPlayerShoot().skipCooldown = true;
        if (Elevator.floorElevatorMap[floorNum].GetComponent<Elevator>().on)
            for (int i = 2; i < 12; i++)
            {
                //Debug.Log(i + " < " + attacks.Length + " < " + watch.songNames.Length + " < " + watch.fileNames.Length);
                attacks[i] = new Attack("Floor " + (i-1), "bonus action", 0, null, 0, 0, null, "Teleports you to floor " + (i-1));
            }
        player.GetComponent<Player>().GetPlayerShoot().SetInfoText();
    }

    public void FixedUpdate()
    {
        if (elevatorSelecting && player.GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            attacks = OgAttacks;
            elevatorSelecting = false;
            player.GetComponent<Player>().GetPlayerShoot().selectedWeapon = OgSel;
            player.GetComponent<Player>().GetPlayerShoot().skipCooldown = true;
            player.GetComponent<Player>().GetPlayerShoot().SetInfoText();
            StartCoroutine(CanElevatorReseet());
        }
    }

    IEnumerator CanElevatorReseet()
    {
        canElevator = false;
        yield return new WaitForSeconds(0.2f);
        canElevator = true;
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
