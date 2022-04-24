using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

    [SerializeField]
    public Camera cam;

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

    private Vector3 initialRot;
    private Vector3 flyRot;

    float roll = 0f;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        initialRot = transform.eulerAngles;
        flyRot = initialRot;
    }

    public void Move(Vector3 vel)
    {
        vel.y += rb.velocity.y;
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
    public Vector3 getThruster()
    {
        return force;
    }
    int fi = 0;
    // Runs every physics iteration
    void FixedUpdate () {
        PerformMovement();
        PerformRotation();
        acceleration = (rb.velocity - prevVel) * Time.fixedDeltaTime;
        prevVel = rb.velocity;
        //print("vel = " + rb.velocity);
        if (acceleration.magnitude > 0.15)
        {
            //Debug.Log("acc = " + acceleration.magnitude);
            GetComponent<Player>().CmdTakeDamage(acceleration.magnitude*60f/(GameManager.instance.matchSettings.moveSpeedMult));
        }
        //if (fi % 5 == 0 && GetComponent<Player>().chara != null && GetComponent<Player>().model != null && (GetComponent<Player>().chara.rotateWithCamera || (GetComponent<Player>().chara.rotateWithCameraWhenFlying && !GetComponent<Player>().IsGrounded())))
        if (fi % 5 == 0 && GetComponent<Player>().chara != null && GetComponent<Player>().model != null)
            GetComponent<Player>().CmdRotate(GetComponent<Player>().model.transform.eulerAngles);
        fi++;
        fi %= 10000;
    }    

    void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            //rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            rb.velocity += MinV(velocity - rb.velocity, 0.2f*(velocity - rb.velocity).normalized);
            //rb.velocity = velocity;
            //Debug.Log("Velocity = " + rb.velocity + " Target Velocity = " + velocity);
            velocity = MaxV(velocity - 0.1f * velocity.normalized, Vector3.zero);
            if (velocity.magnitude < 0.1f)
                velocity = Vector3.zero;
        }
        if (force != Vector3.zero)
        {
            rb.AddForce(force*Time.fixedDeltaTime, ForceMode.Acceleration);
            force = MaxV(force - 0.1f * force.normalized, Vector3.zero);
        }
    }

    private Vector3 MaxV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return a;
        else return b;
        //return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
    }

    private Vector3 MinV(Vector3 a, Vector3 b)
    {
        if (a.magnitude > b.magnitude)
            return b;
        else return a;
        //return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
    }

    void PerformRotation()
    {
        

        if (PauseGame.IsOn)
            return;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        if (cam != null)
        {
            float prevCamRotation = currentCameraRotation;
            currentCameraRotation -= camRotation;
            currentCameraRotation = Mathf.Clamp(currentCameraRotation, -cameraRotationLimit, cameraRotationLimit); // Clamp rotation

            cam.transform.localEulerAngles = new Vector3(currentCameraRotation, 0f, 0f); // Apply rotation to camera
            if (GetComponent<Player>().chara != null && GetComponent<Player>().charId >= 1 && GetComponent<Player>().charId <= 3)
                flyRot += Vector3.Cross(new Vector3(currentCameraRotation - prevCamRotation, 0f, 0f), GetComponent<Player>().chara.rotate.normalized);
            else
                flyRot -= new Vector3(currentCameraRotation - prevCamRotation, 0f, roll);
            if (GetComponent<Player>().chara != null && GetComponent<Player>().model != null && (GetComponent<Player>().chara.rotateWithCamera || (GetComponent<Player>().chara.rotateWithCameraWhenFlying && !GetComponent<Player>().IsGrounded())))
            {
                //GetComponent<Player>().CmdRotate(Vector3.Cross(new Vector3(currentCameraRotation - prevCamRotation, 0f, 0f), GetComponent<Player>().chara.rotate.normalized));
                GetComponent<Player>().model.transform.eulerAngles = new Vector3(flyRot.x, GetComponent<Player>().model.transform.eulerAngles.y, flyRot.z);

            } else if (GetComponent<Player>().chara != null && GetComponent<Player>().chara.rotateWithCameraWhenFlying && GetComponent<Player>().IsGrounded())
            {
                GetComponent<Player>().model.transform.eulerAngles = new Vector3(initialRot.x, GetComponent<Player>().model.transform.eulerAngles.y, initialRot.z);
            }
        }

    }

    public bool canJump()
    {
        if (Mathf.Abs(acceleration.y) > 0.001)
            return false;
        return true;
    }
}
