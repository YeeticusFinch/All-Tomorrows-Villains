using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour {

    const float GROUND_THRESHOLD = 0.5f;

    private bool grounded = false;
    private Player player;

    public bool IsGrounded()
    {
        return grounded;
    }

	// Use this for initialization
	void Start () {
        player = gameObject.GetComponentInParent<Player>();
        /*if (gameObject.GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //gameObject.AddComponent<FixedJoint>();
            //gameObject.GetComponent<FixedJoint>().connectedBody = player.gameObject.GetComponent<Rigidbody>();
            //gameObject.GetComponent<FixedJoint>().connectedAnchor = transform.localPosition;
            //gameObject.GetComponent<Rigidbody>().useGravity = false;
        }*/
	}


    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("GroundControl touching collider");
        if (!grounded)
            foreach (ContactPoint c in collision.contacts)
                if (Vector3.Dot(c.normal, Vector3.up) > GROUND_THRESHOLD)
                    grounded = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("GroundControl hit collider");
        if (!grounded)
        {
            foreach (ContactPoint c in collision.contacts)
            {
                Debug.Log("c.normal = " + c.normal);
                if (Vector3.Dot(c.normal, Vector3.up) > GROUND_THRESHOLD)
                    grounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("GroundControl left collider");
        if (grounded)
            foreach (ContactPoint c in collision.contacts)
                if (Vector3.Dot(c.normal, Vector3.up) > GROUND_THRESHOLD)
                    grounded = false;
    }
}

