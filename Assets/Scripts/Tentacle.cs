using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour {

    public int length;
    public LineRenderer lr;
    public Vector3[] segmentPoses;
    public Vector3[] segmentV;

    public Transform targetDir;
    public float targetDist;
    public float smoothSpeed;
    
	void Start () {
        lr.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
	}
	
	void Update () {
        segmentPoses[0] = targetDir.position;

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i-1] + targetDir.right * targetDist, ref segmentV[i], smoothSpeed);
        }
        lr.SetPositions(segmentPoses);
	}
}
