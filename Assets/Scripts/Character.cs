using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [System.Serializable]
    public struct SavingModifier
    {
        public string type;
        public int mod;
    }
    [System.Serializable]
    public struct DamageEffect
    {
        public string damage;
        public char type;
    }

    public int id;
    public string title = "Monster";
    public string size = "medium";
    public int AC = 9;
    public int ACMagicBonus = 0;
    public int HP = 10;
    public int WALK_SPEED = 30;
    public int CLIMB_SPEED = 0;
    public int FLY_SPEED = 0;
    public bool HOVER = false;
    public float CR = 0;
    public DamageEffect[] damageEffectsStruct;
    public SavingModifier[] savingModifiersStruct;
    public Dictionary<string, char> damageEffects = new Dictionary<string, char>();
    public Dictionary<string, int> savingModifiers = new Dictionary<string, int>();
    //public string[] resistances;
    //public string[] vulnerabilities;
    //public string[] immunities;
    //public string[] reverseDamages;
    //public float attackSpeed = 1f;
    public float jumpHeight;
    public int fov = 60;
    public Creature creature;
    public Vector3 rotate;
    public Vector3 offset;
    //public GameObject[] canJumpFrom;
    public ParticleSystem[] particles;
    public GameObject[] tintable;
    public GameObject[] primaryEmitters;
    public GameObject[] secondaryEmitters;
    public GameObject[] tertiaryEmitters;
    public GameObject[] hideFirstPerson;
    public GameObject[] hideThirdPerson;
    public GameObject[] shrinkFirstPerson;
    public GameObject[] canJumpFrom;
    public GameObject camAttach;
    public GameObject camAttachTo;
    public bool camAttachToRotate = true;
    public GameObject[] spinWithCamera;
    public float spinWithCameraScalar = 1;
    public bool rotateWithCamera = false;
    public bool rotateWithCameraWhenFlying = false;
    public Vector3 rotateWithCamScalars = new Vector3(1, 1, 1);
    //public Vector3 rotateWithCamOffset = new Vector3(0, 0, 0);
    public bool lerpRotate = false;
    public float distanceToGround = 0.5f;
    public Vector3 cameraOffset;

    public void Awake()
    {
        foreach (SavingModifier e in savingModifiersStruct)
            savingModifiers.Add(e.type, e.mod);
        foreach (DamageEffect e in damageEffectsStruct)
            damageEffects.Add(e.damage, e.type);
    }

    public int GetSave(string s)
    {
        if (savingModifiers.ContainsKey(s))
            return savingModifiers[s];
        return 0;
    }

    public float GetDamageMod(string damageType)
    {
        if (damageEffects.ContainsKey(damageType))
        {
            switch (damageEffects[damageType])
            {
                case 'r':
                    return 0.5f;
                case 'v':
                    return 2;
                case 'i':
                    return 0;
                case 'R':
                    return 0.25f;
                case 'V':
                    return 4;
                case 'I':
                    return -0.5f;
                case 'h':
                    return -1;
                case 'H':
                    return -2;
            }
        }
        return 1;
    }
}
