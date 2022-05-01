using UnityEngine;
using System.Collections;

public class CarlMath
{/*
    public static Vector3 unitVectorFromEuler(Vector3 a)
    {
        Vector3 result = new Vector3(;

        return result;
    }*/

    public static float MaxMag(float a, float b)
    {
        return Mathf.Abs(a) > Mathf.Abs(b) ? a : b;
    }

    public static float MinMag(float a, float b)
    {
        return Mathf.Abs(a) < Mathf.Abs(b) ? a : b;
    }

    public static float angleFix(float a)
    {
        a %= 360;
        if (a > 180) a -= 360;
        return a;
    }

    public static GameObject[] arrayCombine(GameObject[] arr0, GameObject[] arr1)
    {
        GameObject[] result = new GameObject[arr0.Length + arr1.Length];

        for (int i = 0; i < arr0.Length; i++)
            result[i] = arr0[i];

        for (int i = 0; i < arr1.Length; i++)
            result[arr0.Length + i] = arr1[i];

        return result;
    }

    public static Vector3 MaxV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return a;
        else return b;
        //return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
    }

    public static Vector3 MinV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return b;
        else return a;
        //return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
    }

    public static int modClamp(int a, int b)
    {
        a %= b;
        while (a < 0)
            a += b;
        return a;
    }
}
