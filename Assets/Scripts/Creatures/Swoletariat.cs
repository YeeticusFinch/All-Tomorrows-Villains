using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Swoletariat : Creature {

    Vector3 angle = Vector3.zero;
    Vector3 headOffset = Vector3.zero;

    [System.Serializable]
    public struct Limb
    {
        public string name;
        public GameObject[] seg;
        public int state;
        public Vector3[] ogRot;
        public Vector3[] ogPos;
    }

    public int spineNum;
    public int tentacleNum;
    public int headNum;
    public int fistNum;
    public int bladeNum;

    public Limb[] limbs;

    public override void syncCreatureInstance(Creature c)
    {
        brusilov = ((Swoletariat)c).brusilov;
    }

    public override float[] getCreatureDataFloats()
    {
        return base.getCreatureDataFloats();
    }

    public override int[] getCreatureDataInts()
    {
        return new int[] { brusilov };
        //return base.getCreatureDataInts();
    }

    public override void syncCreatureData(float[] floats, int[] ints)
    {
        brusilov = ints[0];
        base.syncCreatureData(floats, ints);
    }

    // Use this for initialization
    void Start () {
        for (int i = 0; i < limbs.Length; i++)
        {
            limbs[i].ogRot = new Vector3[limbs[i].seg.Length];
            limbs[i].ogPos = new Vector3[limbs[i].seg.Length];
            for (int j = 0; j < limbs[i].seg.Length; j++)
            {
                limbs[i].ogRot[j] = limbs[i].seg[j].transform.localEulerAngles;
                limbs[i].ogPos[j] = limbs[i].seg[j].transform.localPosition;
            }
        }
		//foreach (Limb l in limbs)
        //{
        //    l.ogRot = new Vector3[l.seg.Length]();

        //    if (l.name == "leg")
        //        l.seg[2].GetComponent<SphereCollider>().enabled = false;
        //}
	}

    [Client]
    void Shoot(int num, string save, float damage, string dmgType, float range)
    {
        range = 2.5f + range/3;
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

    int brusilov = 0;
    public override float primary(int sel)
    {
        Attack a = attacks[sel];
        Limb l;
        switch (sel)
        {
            case 0: // Tokarev - Pistol attack +5 to hit, 60/240 feet, 6.5 piercing, 3/turn
                Shoot(a.num, a.save, a.damage, a.damageType, a.range);
                return 3;
            case 1: // Red Arm(y) - Sword attack +5 to hit, 10 feet, hit 8 slashing, 2/turn
                Shoot(a.num, a.save, a.damage, a.damageType, a.range);
                l = limbs[bladeNum];
                StartCoroutine(rotateToAndBack(l.seg[0], l.seg[1], l.ogRot[0] - 120 * Vector3.up - 10 * Vector3.right, l.ogRot[1] + 40 * Vector3.right, l.ogRot[0], l.ogRot[1], 1.5f, 13));
                return 2;
            case 2: // Brusilov Offensive - arms and bite attack, 4/turn
                switch (brusilov)
                {
                    case 0: // Blade (same as "Red Arm(y)")
                        Shoot(5, null, 8, "slashing", a.range);
                        l = limbs[bladeNum];
                        StartCoroutine(rotateToAndBack(l.seg[0], l.seg[1], l.ogRot[0] - 120 * Vector3.up - 10 * Vector3.right, l.ogRot[1] + 40 * Vector3.right, l.ogRot[0], l.ogRot[1], 1.5f, 13));
                        brusilov = 1;
                        return 4;
                    case 1: // Punch +8 to hit, 7.5 bludgeoning
                        Shoot(8, null, 7, "bludgeoning", a.range);
                        l = limbs[fistNum];
                        StartCoroutine(rotateToAndBack(l.seg[0], l.seg[1], l.ogRot[0] - 40 * Vector3.up - 10 * Vector3.right, l.ogRot[1] - 40 * Vector3.right, l.ogRot[0], l.ogRot[1], 4, 5));
                        brusilov = 2;
                        return 4;
                    case 2: // Bite +5 to hit, 4.5 piercing
                        Shoot(5, null, 4, "piercing", a.range);
                        l = limbs[headNum];
                        //StartCoroutine(moveToAndBack(l.seg[0], l.ogPos[0] + Vector3.forward * (1.5f + a.range / 3), l.ogPos[0], 0.1f, 1.5f));
                        StartCoroutine(moveHeadToAndBack(cam.transform.forward * (1.5f + a.range / 3), Vector3.zero, 0.5f, l.seg[0], 2));
                        brusilov = 0;
                        return 4;
                }
                break;
            case 3: // Avtobroneotryady - Tentacle attack +8 to hit, 15 feet, 8.5 bludgeoning, 10 foot knockback, 3/turn
                break;
            case 4: // Revolutionary Objection - shimmering psychic lance from forehead 120 feet, DC 16 Int save, 35 psychic + 6 seconds incapacitation, 1.5/turn, (recharge 4-6)
                break;
            case 5: // Psychological Emancipation - spike of psychic energy 60 feet, DC 16 Int save, 10.5 psychic
                break;
        }
        return 0;
    }

    public override void walkAnim(float speed, Vector3 dir)
    {
        Debug.Log("Walk = " + speed);
        angle -= 2*speed * Vector3.Cross(dir, Vector3.up);
        angle = new Vector3(angle.x, 0, angle.z); //redundant
        limbs[spineNum].seg[0].transform.eulerAngles = angle;
        //base.walkAnim(speed);
    }

    public override void fallAnim(float speed, Vector3 dir)
    {
        walkAnim(speed, dir);
        //base.fallAnim(speed, dir);
    }

    // Update is called once per frame
    void Update () {
        limbs[headNum].seg[0].transform.position = limbs[spineNum].seg[3].transform.position + 0.6f*limbs[spineNum].seg[3].transform.up + 0.3f * limbs[spineNum].seg[3].transform.forward + headOffset;
        Vector3 contactPos = (player != null && player.GetComponent<Player>() != null) ? player.GetComponent<Player>().groundContactPos : Vector3.zero;
        if (Input.GetButton("Jump"))
        {
            foreach (Limb l in limbs)
                if (l.name == "leg" || l.name == "arm")
                    rotateToStep(l.seg[0], l.seg[1], l.ogRot[0], l.ogRot[1], 2);
        }
        else
            foreach (Limb l in limbs)
                if (l.name == "leg" || l.name == "arm")
                    twoPoint2Step(l.seg[0], l.seg[1], l.seg[2], l.ogRot[0], l.ogRot[1], new Vector3(l.seg[0].transform.position.x, contactPos.y, l.seg[0].transform.position.z), 8, 40, 40);
        dir += (int)Mathf.Sign(dir);
        if (Mathf.Abs(dir) > 40) dir = -1 * (int)Mathf.Sign(dir);
    }

    bool rotateToStep(GameObject seg0, GameObject seg1, Vector3 angle0, Vector3 angle1, float step = 0.8f, float scaleStep = 0)
    {
        seg0.transform.localEulerAngles += CarlMath.MinV(angle0 - seg0.transform.localEulerAngles, step * (angle0 - seg0.transform.localEulerAngles).normalized);
        seg1.transform.localEulerAngles += CarlMath.MinV(angle1 - seg1.transform.localEulerAngles, step * (angle1 - seg1.transform.localEulerAngles).normalized);
        seg0.transform.localScale += new Vector3(scaleStep, scaleStep, scaleStep);
        seg1.transform.localScale += new Vector3(scaleStep, scaleStep, scaleStep);
        return (seg0.transform.localEulerAngles - angle0).magnitude + (seg1.transform.localEulerAngles - angle1).magnitude < 0.1f * step;
    }

    IEnumerator rotateTo(GameObject seg0, GameObject seg1, Vector3 angle0, Vector3 angle1, float step = 0.8f)
    {
        while (!rotateToStep(seg0, seg1, angle0, angle1, step))
            yield return new WaitForFixedUpdate();
    }

    IEnumerator rotateToAndBack(GameObject seg0, GameObject seg1, Vector3 angle0, Vector3 angle1, Vector3 ogAngle0, Vector3 ogAngle1, float scale = 1, float step = 0.8f)
    {
        float scaleStep = 0;
        if (scale != 1)
            scaleStep = (scale - 1) / ( Mathf.Max((angle0 - seg0.transform.localEulerAngles).magnitude, (angle1 - seg1.transform.localEulerAngles).magnitude) / step );
        while (!rotateToStep(seg0, seg1, angle0, angle1, step, scaleStep))
            yield return new WaitForFixedUpdate();
        while (!rotateToStep(seg0, seg1, ogAngle0, ogAngle1, step, -scaleStep))
            yield return new WaitForFixedUpdate();
    }

    IEnumerator twoPoint(GameObject seg0, GameObject seg1, GameObject seg2, Vector3 ogAngle0, Vector3 ogAngle1, Vector3 destination, float step = 8f, float waitTillRetreat = 0.2f, bool retreat = false, float retreatStep = 0.8f, float maxAngleA = 100, float maxAngleB = 120)
    {
        while (!twoPoint2Step(seg0, seg1, seg2, ogAngle0, ogAngle1, destination, step, maxAngleA, maxAngleB))
            yield return new WaitForFixedUpdate();
        if (retreat)
        {
            yield return new WaitForSeconds(waitTillRetreat);
            StartCoroutine(rotateTo(seg0, seg1, ogAngle0, ogAngle1, retreatStep));
        }
    }

    IEnumerator moveToAndBack(GameObject seg, Vector3 pos, Vector3 ogPos, float step = 0.1f, float scale = 1)
    {
        float scaleStep = 0;
        if (scale != 1)
            scaleStep = (scale - 1) / (((pos - seg.transform.localPosition).magnitude) / step);
        while ((seg.transform.localPosition - pos).magnitude > step)
        {
            seg.transform.localPosition += CarlMath.MinV((pos - seg.transform.localPosition), step * (pos - seg.transform.localPosition).normalized);
            seg.transform.localScale += Vector3.one * scaleStep;
            yield return new WaitForFixedUpdate();
        }
        while ((seg.transform.localPosition - ogPos).magnitude > step)
        {
            seg.transform.localPosition += CarlMath.MinV((ogPos - seg.transform.localPosition), step * (ogPos - seg.transform.localPosition).normalized);
            seg.transform.localScale -= Vector3.one * scaleStep;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator moveHeadToAndBack(Vector3 pos, Vector3 ogPos, float step = 0.1f, GameObject segToScale = null, float scale = 1)
    {
        limbs[headNum].seg[1].GetComponent<SphereCollider>().isTrigger = true;
        float scaleStep = 0;
        if (scale != 1 && segToScale != null)
            scaleStep = (scale - 1) / (((pos - segToScale.transform.localPosition).magnitude) / step);
        while ((headOffset - pos).magnitude > step)
        {
            headOffset += CarlMath.MinV((pos - headOffset), step * (pos - headOffset).normalized);
            segToScale.transform.localScale += Vector3.one * scaleStep;
            yield return new WaitForFixedUpdate();
        }
        while ((headOffset - ogPos).magnitude > step*0.1f)
        {
            headOffset += CarlMath.MinV((ogPos - headOffset), step * (ogPos - headOffset).normalized);
            segToScale.transform.localScale -= Vector3.one * scaleStep;
            yield return new WaitForFixedUpdate();
        }
        limbs[headNum].seg[1].GetComponent<SphereCollider>().isTrigger = false;
    }

    // Returns true if it is still moving
    int dir = 1;
    bool twoPoint2Step(GameObject seg0, GameObject seg1, GameObject seg2, Vector3 ogAngle0, Vector3 ogAngle1, Vector3 destination, float step = 8f, float maxAngleA = 100, float maxAngleB = 120)
    {
        // Get lengths of segments
        float a = (seg1.transform.position - seg0.transform.position).magnitude;
        float b = (seg2.transform.position - seg1.transform.position).magnitude;
        if ((seg2.transform.position - destination).magnitude < step * 0.1f) // We're already in position bois
        {
            if ((ogAngle0 - seg0.transform.localEulerAngles).magnitude < step)
                seg0.transform.localEulerAngles += CarlMath.MinV(ogAngle0 - seg0.transform.localEulerAngles, step * (ogAngle0 - seg0.transform.localEulerAngles).normalized);
            else
                seg0.transform.localEulerAngles += Vector3.right * Mathf.Sign(dir) * 0.1f;
            if ((ogAngle1 - seg1.transform.localEulerAngles).magnitude < step)
                seg1.transform.localEulerAngles += CarlMath.MinV(ogAngle1 - seg1.transform.localEulerAngles, step * (ogAngle1 - seg1.transform.localEulerAngles).normalized);
            else
                seg1.transform.localEulerAngles -= Vector3.right * Mathf.Sign(dir) * 0.1f;
            return false;
        }
        //step *= 10;
        for (int i = 0; i < 4; i++)
        {
            Vector3 startAngle0 = seg0.transform.localEulerAngles;
            Vector3 startAngle1 = seg1.transform.localEulerAngles;
            Vector3 startPos1 = seg1.transform.position;
            Vector3 startPos2 = seg2.transform.position;

            float ogBD = (seg1.transform.position - destination).magnitude; // Distance between knee joint and destination, target = b
            float ogCD = (seg2.transform.position - destination).magnitude; // Distance between foot and destination, target = 0

            bool bool1 = Mathf.Abs((seg1.transform.position - destination).magnitude - b) > step * 0.1f; // not touching ground
            bool bool2 = Mathf.Abs((seg1.transform.position - destination).magnitude - b) < 1.05f * (a + b); // close enough to touch ground

            // Let's try moving the thigh
            if (bool1 && bool2)
            {
                seg0.transform.localEulerAngles += Vector3.right * step * 0.1f;
                if (Mathf.Abs((seg1.transform.position - destination).magnitude - b) > Mathf.Abs(ogBD - b))
                {
                    seg0.transform.localEulerAngles -= 2 * Vector3.right * step * 0.1f; // Whoops we went the wrong way
                    if (Mathf.Abs((seg1.transform.position - destination).magnitude - b) > Mathf.Abs(ogBD - b))
                        seg0.transform.localEulerAngles += Vector3.right * step * 0.1f; // Turns out we didn't need to move at all
                }
                if ((startPos1 - seg1.transform.position).magnitude > 1) // Make sure we don't rotate by an extreme amount
                    seg0.transform.localEulerAngles = startAngle0;
            } else if (!bool2)
            {
                if ((ogAngle0 - seg0.transform.localEulerAngles).magnitude < step)
                    seg0.transform.localEulerAngles += CarlMath.MinV(ogAngle0 - seg0.transform.localEulerAngles, step * (ogAngle0 - seg0.transform.localEulerAngles).normalized);
                else
                    seg0.transform.localEulerAngles += Vector3.right * Mathf.Sign(dir) * 0.1f;
            } else
            {
                if ((ogAngle0 - seg0.transform.localEulerAngles).magnitude < step)
                    seg0.transform.localEulerAngles += CarlMath.MinV(ogAngle0 - seg0.transform.localEulerAngles, step * (ogAngle0 - seg0.transform.localEulerAngles).normalized);
                else
                    seg0.transform.localEulerAngles += Vector3.right * Mathf.Sign(dir) * 0.1f;
            }

            bool bool11 = (seg2.transform.position - destination).magnitude > step * 0.1f; // not touching ground
            bool bool22 = (seg2.transform.position - destination).magnitude < 1.05f * (a + b); // close enough to touch ground

            // Let's try moving the calf
            if (bool11 && bool22)
            {
                seg1.transform.localEulerAngles += Vector3.right * step * 0.1f;
                if ((seg2.transform.position - destination).magnitude > ogCD)
                {
                    seg1.transform.localEulerAngles -= 2 * Vector3.right * step * 0.1f; // Whoops we went the wrong way
                    if ((seg2.transform.position - destination).magnitude > ogCD)
                        seg1.transform.localEulerAngles += Vector3.right * step * 0.1f; // Turns out we didn't need to move at all
                }
                if ((startPos2 - seg2.transform.position).magnitude > 1) // Make sure we don't rotate by an extreme amount
                    seg1.transform.localEulerAngles = startAngle1;
            }
            else if (!bool22)
            {
                if ((ogAngle1 - seg1.transform.localEulerAngles).magnitude < step)
                    seg1.transform.localEulerAngles += CarlMath.MinV(ogAngle1 - seg1.transform.localEulerAngles, step * (ogAngle1 - seg1.transform.localEulerAngles).normalized);
                else
                    seg1.transform.localEulerAngles += Vector3.right * Mathf.Sign(dir) * 0.1f;
            }
            else
            {
                if ((ogAngle1 - seg1.transform.localEulerAngles).magnitude < step)
                    seg1.transform.localEulerAngles += CarlMath.MinV(ogAngle1 - seg1.transform.localEulerAngles, step * (ogAngle1 - seg1.transform.localEulerAngles).normalized);
                else
                    seg1.transform.localEulerAngles -= Vector3.right * Mathf.Sign(dir) * 0.1f;
            }
            // Make sure we don't go over the max
            seg0.transform.localEulerAngles = new Vector3(Mathf.Clamp(seg0.transform.localEulerAngles.x, ogAngle0.x - maxAngleA, ogAngle0.x + maxAngleA), startAngle0.y, startAngle0.z);
            seg1.transform.localEulerAngles = new Vector3(Mathf.Clamp(seg1.transform.localEulerAngles.x, ogAngle1.x - maxAngleB, ogAngle1.x + maxAngleB), startAngle1.y, startAngle1.z);
            if (!bool1 && !bool11) return false;
        }
        return false;
    }

    // Ok maybe this function is too complicated lmao
    void twoPointOld(GameObject seg0, GameObject seg1, GameObject seg2, Vector3 ogAngle0, Vector3 ogAngle1, Vector3 destination, float step = 2, float maxAngleA = 100, float maxAngleB = 120)
    {
        // Imagine the segments are a leg, seg0 is the thigh, seg1 is the calf, destination is the desired foot location
        //float a = (new Vector3(0, seg1.transform.position.y - seg0.transform.position.y, seg1.transform.position.z - seg0.transform.position.z)).magnitude;

        // Get lengths of segments
        float a = (seg1.transform.position - seg0.transform.position).magnitude;
        float b = (seg2.transform.position - seg1.transform.position).magnitude;

        // Get distance from beginning of thigh to destination
        float c = (new Vector3(seg0.transform.position.x, destination.y, destination.z) - seg0.transform.position).magnitude;

        // Get OG positions
        Vector3 posA = seg0.transform.position;
        Vector3 posB = seg0.transform.position + Quaternion.Euler(ogAngle0) * Vector3.forward * a;
        Vector3 posC = posB + Quaternion.Euler(ogAngle1) * Vector3.forward * b;
        
        float angleA = Mathf.Clamp(CarlMath.cosLawAngle(a, c, b), -maxAngleA, maxAngleA); // Angle between seg0 and destination-seg0
        float angleB = Mathf.Clamp(CarlMath.cosLawAngle(b, a, c), 180-maxAngleB, 180); //Angle between seg0 and seg1
        float angleD = Mathf.Atan((destination.z - posA.z) / (destination.y - posA.y)); // Angle in real world space from thigh to destination

        float angleAf = ogAngle0.x - angleD + angleA; // Angle in world space
        //float angleBf
        //seg0.transform.localRotation += (seg0.transform.localRotation)
    }
}
