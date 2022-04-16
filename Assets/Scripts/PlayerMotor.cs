using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    public Rigidbody rb;
    private Vector3 rotation = Vector3.zero;
    private float camRotation = 0;
    private Vector3 force = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;
    private float currentCameraRotation = 0f;

    private Vector3 acceleration = Vector3.zero;
    private Vector3 prevVel = Vector3.zero;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}

    public void Move(Vector3 vel)
    {
        velocity = vel;
    }

    public void Rotate(Vector3 rot)
    {
        rotation = rot;
    }

    public void CamRotate(float rot)
    {
        camRotation = rot;
    }

    public void ApplyThruster(Vector3 f)
    {
        force = f;
    }

    // Runs every physics iteration
    void FixedUpdate () {
        PerformMovement();
        PerformRotation();
        acceleration = (rb.velocity - prevVel) * Time.fixedDeltaTime;
        prevVel = rb.velocity;
        //print("vel = " + rb.velocity);
        //if (Input.GetButton("Jump")) print("acc = " + acceleration);
	}

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
        if (force != Vector3.zero)
        {
            rb.AddForce(force*Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }

    void PerformRotation()
    {
        if (PauseGame.IsOn)
            return;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            currentCameraRotation -= camRotation;
            currentCameraRotation = Mathf.Clamp(currentCameraRotation, -cameraRotationLimit, cameraRotationLimit); // Clamp rotation

            cam.transform.localEulerAngles = new Vector3(currentCameraRotation, 0f, 0f); // Apply rotation to camera
        }
    }

    public bool canJump()
    {
        if (Mathf.Abs(acceleration.y) > 0.001)
            return false;
        return true;
    }
}
