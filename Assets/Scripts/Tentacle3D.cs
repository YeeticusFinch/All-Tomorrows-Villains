using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle3D : MonoBehaviour {

    public int length;
    public TubeRenderer tr;
    public Vector3[] segmentPoses;
    public Vector3[] segmentV;
    public GameObject endPoint;
    public GameObject superParent;

    public Transform targetDir;
    public float targetDist;
    public float smoothSpeed;

    public Vector3 startLean = new Vector3(0, 0, 0);
    public Vector3 targetLean = new Vector3(0, 0, 0);
    private Vector3 slm = Vector3.zero;
    private Vector3 tlm = Vector3.zero;
    private Vector3 slmd = Vector3.zero;
    private Vector3 tlmd = Vector3.zero;
    //public Transform beginLean;
    //public Transform endLean;

    public float yeet = 2f;
    public float yeetStart = 1f;
    public float yeetEnd = 1f;

    Vector3 eyeRot = new Vector3(0f, 90f, 0f);
    Vector3 eyeDir = new Vector3(0f, 90f, 0f);

    void Start()
    {
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
        tr.SetPositions(segmentPoses);
        endPoint.transform.localScale *= 1.4f;
        //beginLean.localPosition = startLean;
        //endLean.localPosition = targetLean;
    }

    void Update()
    {
        //startLean = beginLean.position - targetDir.position;
        //targetLean = endLean.position - targetDir.position;
        if (Random.Range(0, 5) == 0)
        {
            slmd = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5))*0.1f;
            tlmd = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5))*0.1f;
        }
        slm = new Vector3(Mathf.Clamp(slm.x+slmd.x, -8, 8), Mathf.Clamp(slm.y + slmd.y, -8, 8), 0.1f*Mathf.Clamp(slm.z + slmd.z, -8, 8));
        tlm = new Vector3(Mathf.Clamp(tlm.x+tlmd.x, -8, 8), Mathf.Clamp(tlm.y + tlmd.y, -8, 8), 0.1f*Mathf.Clamp(tlm.z + tlmd.z, -8, 8));

        Vector3 lean0 = RotatePointAroundPivot(startLean + slm, Vector3.zero, superParent.transform.rotation.eulerAngles);
        Vector3 lean1 = RotatePointAroundPivot(targetLean - tlm, Vector3.zero, superParent.transform.rotation.eulerAngles);

        //beginLean.transform.position = lean0 + targetDir.position;
        //endLean.transform.position = lean1 + targetDir.position;

        segmentPoses[0] = targetDir.position;

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + (yeet*targetDir.right + (yeetStart*lean0*(segmentPoses.Length-i) + yeetEnd* lean1 * Mathf.Pow(1.1f, i)) / segmentPoses.Length).normalized * targetDist, ref segmentV[i], smoothSpeed);
        }
        if (endPoint != null)
        {
            endPoint.transform.position = segmentPoses[segmentPoses.Length - 1];
            endPoint.transform.rotation = Quaternion.FromToRotation(Vector3.up, segmentPoses[segmentPoses.Length - 1] - segmentPoses[segmentPoses.Length - 2]);
            if (Random.Range(0, 5) == 0) eyeRot = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
            eyeDir += eyeRot;
            eyeDir = new Vector3(Mathf.Clamp(eyeDir.x, -40, 40), Mathf.Clamp(eyeDir.y, 50, 130), Mathf.Clamp(eyeDir.z, -40, 40));
            endPoint.transform.eulerAngles += eyeDir;
        }
        tr.SetPositions(segmentPoses);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
