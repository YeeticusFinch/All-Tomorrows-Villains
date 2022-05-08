
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

    //[SerializeField]
    //private float speed = 10f; // 10 --> 45 feet per turn
    private float walkSpeed = 0f;
    private float climbSpeed = 0f;
    private float flySpeed = 0f;
    [SerializeField]
    private float mouseSensitivity = 3f;
    //[SerializeField]
    private float jumpForce;
    private float zoom3 = 1f;
    [SerializeField]
    private GameObject orienter;

    [SerializeField]
    private LayerMask maskCam3p;

    //private bool canJump = true;
    //private bool canFly = false;

    private PlayerMotor motor;

    private Character chara;

    public Creature creature;

    [SerializeField]
    private LayerMask jumpMask;

    void Start () {
        motor = GetComponent<PlayerMotor>();
	}

    public void Setup(float ws, float cs, float fs, float jf)
    {
        chara = GetComponent<Player>().chara;
        walkSpeed = ws;
        climbSpeed = cs;
        flySpeed = fs;
        jumpForce = jf;
        //walkSpeed = (chara.WALK_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult * 1.17f * 1.55f;
        //climbSpeed = (chara.CLIMB_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult;
        //flySpeed = (chara.FLY_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult * GameManager.instance.matchSettings.moveSpeedMult;
        //jumpForce = chara.jumpHeight * GameManager.instance.matchSettings.moveSpeedMult * 10f;
    }
    private float currCam3Rot = 0f;
    private Vector3 cam3Dir = Vector3.zero;
	void Update () {

        // If paused, return
        if (PauseGame.IsOn)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

        // Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxisRaw("Horizontal");
        float yMov = (Input.GetButton("Jump") ? 1 : 0) - (Input.GetButton("Crouch") ? 1 : 0);
        //Debug.Log("yMov = " + yMov);
        float zMov = Input.GetAxisRaw("Vertical");
        Vector3 velocity = Vector3.zero;
        bool maxSpeed = false;
        float speed = walkSpeed;
        if (Mathf.Abs(xMov) + Mathf.Abs(yMov) + Mathf.Abs(zMov) > 0) {
            //Final Movement vector
            maxSpeed = true;
            if (!IsGrounded()) speed = flySpeed == 0 ? walkSpeed/2f : flySpeed;
            speed *= Input.GetButton("Sprint") ? 2 : 1;
            if (speed == flySpeed * (Input.GetButton("Sprint") ? 2 : 1))
            {
                Vector3 movDir = ((Quaternion.AngleAxis(-motor.roll, motor.cam.transform.forward) * motor.cam.transform.right) * xMov + motor.cam.transform.up * yMov + motor.cam.transform.forward * zMov).normalized;
                //float tempSpeed = Mathf.Min((movDir*speed - motor.rb.velocity).magnitude, speed);
                //velocity += movDir * tempSpeed;
                velocity += CarlMath.MinV(movDir * speed - motor.rb.velocity, speed * (movDir * speed - motor.rb.velocity).normalized);
                //creature.flyAnim((Input.GetButton("Sprint") ? 2 : 1) * zMov / Mathf.Abs(zMov));
            }
            else
            {
                float tempSpeed = Mathf.Min(speed - motor.rb.velocity.magnitude, speed*4);
                velocity += (transform.right * xMov + transform.forward * zMov).normalized * tempSpeed;
                //Debug.Log("Walking vel = " + velocity);
                //velocity /= 1 + Mathf.Max(0, (motor.rb.velocity + velocity).magnitude - speed) / speed;
                //creature.walkAnim((Input.GetButton("Sprint") ? 2 : 1)*zMov/Mathf.Abs(zMov));
            }
            
            //Debug.Log("Max Speed = " + speed + ", rb.velocity = " + motor.rb.velocity.magnitude + ", velocity = " + velocity.magnitude + ", combined = " + (motor.rb.velocity + velocity).magnitude);

            /*
            if (motor.rb.velocity.x + velocity.x > speed) velocity.x = speed - motor.rb.velocity.x;
            if (motor.rb.velocity.x + velocity.x < -speed) velocity.x = -speed - motor.rb.velocity.x;
            if (motor.rb.velocity.z + velocity.z > speed) velocity.z = speed - motor.rb.velocity.z;
            if (motor.rb.velocity.z + velocity.z < -speed) velocity.z = -speed - motor.rb.velocity.z;*/
        }
        //Apply movement
        //motor.Move(velocity);

        //Calculate rotation as a 3D vector (turning the player around)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * mouseSensitivity;
        //Apply rotation
        if (!GameManager.instance.freeCam)
            motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vector (turning the camera)
        float xRot = Input.GetAxisRaw("Mouse Y");
        //float zRot = Input.GetAxisRaw("Mouse Z");

        float camRotation = xRot * mouseSensitivity;
        //Apply rotation
        if (!GameManager.instance.freeCam)
            motor.CamRotate(camRotation);
        else
        {//motor.CamRotate3(camRotation);
            zoom3 *= 1-0.02f*Input.GetAxis("3rd Person Zoom");
            //orienter.transform.eulerAngles += rotation;
            currCam3Rot = Mathf.Clamp(currCam3Rot - camRotation, -motor.cameraRotationLimit, motor.cameraRotationLimit);
            orienter.transform.localEulerAngles = Vector3.right * currCam3Rot + Vector3.up * (yRot * mouseSensitivity + orienter.transform.localEulerAngles.y);
            //cam3Dir = new Vector3(cam3Dir.x + yRot*mouseSensitivity, Mathf.Clamp(cam3Dir.y + xRot * mouseSensitivity, -motor.cameraRotationLimit, motor.cameraRotationLimit), 0);
            RaycastHit hit;
            //Effects.instance.Sparky(motor.cam.transform.position, motor.cam.transform.position + orienter.transform.forward * zoom3 * 3f, null, null);
            if (Physics.Raycast(transform.position+0.6f*(motor.cam.transform.position-transform.position) + 0.2f*Vector3.up, orienter.transform.forward, out hit, zoom3*3f, maskCam3p))
            {
                GetComponent<Player>().cam3.transform.position = hit.point;
            } else
            {
                GetComponent<Player>().cam3.transform.position = transform.position + 0.6f * (motor.cam.transform.position - transform.position) + 0.2f * Vector3.up + orienter.transform.forward * zoom3 * 3f;
            }
            //GetComponent<Player>().cam3.transform.eulerAngles = orienter.transform.eulerAngles;
            GetComponent<Player>().cam3.transform.localEulerAngles = -Vector3.right * currCam3Rot + (180 + orienter.transform.localEulerAngles.y) * Vector3.up;
        }

        if (Input.GetButton("Jump") && IsGrounded())
        {
            //if (maxSpeed)
            //    velocity /= 1 + Mathf.Max(0, (motor.rb.velocity + velocity).magnitude - speed) / speed;
            //velocity.y = jumpForce*10000000*3.15f;
            motor.ApplyThruster(Vector3.up*jumpForce*1000);
        } /*else if ((Input.GetButton("Jump") || Input.GetButton("Crouch")) && flySpeed > 0)
        {
            maxSpeed = true;
            velocity += (Input.GetButton("Crouch") ? -1 : 1) * motor.cam.transform.up.normalized * flySpeed;
            velocity /= 1 + Mathf.Max(0, (motor.rb.velocity + velocity).magnitude - speed) / speed;
        }*/
        if (motor.getThruster().magnitude > 0 && !(Input.GetButton("Jump") && grounded))
        {
            motor.ApplyThruster(Vector3.zero);
        }
        //velocity *= Input.GetButton("Sprint") ? 2 : 1;
        //motor.ApplyThruster(velocity*100);
        if (grounded) motor.Move(velocity);
        else
        {
            //Debug.Log("not grounded");
            motor.ApplyThruster(velocity * 100);
        }
        
    }

    bool grounded = false;

    bool IsGrounded() {
        grounded = GetComponent<Player>().IsGrounded();
        return grounded;
        RaycastHit hit;
        //int i = 0;
        foreach (GameObject e in GetComponent<Player>().chara.canJumpFrom)
            if (e.GetComponent<SphereCollider>() != null)
            {
                Effects.instance.CmdSparky2(e.transform.position + e.GetComponent<SphereCollider>().center * e.transform.localScale.magnitude, e.transform.position + e.GetComponent<SphereCollider>().center * e.transform.localScale.magnitude - Vector3.up*(e.GetComponent<SphereCollider>().radius * e.GetComponent<SphereCollider>().transform.localScale.magnitude + 0.1f), 0, null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<SphereCollider>().center * e.transform.localScale.magnitude, -Vector3.up, out hit, e.GetComponent<SphereCollider>().radius*e.GetComponent<SphereCollider>().transform.localScale.magnitude + 0.1f, jumpMask);
                //bool yeet = Physics.Ray
                if (yeet)
                {
                    GameObject t = hit.collider.gameObject;
                    Effects.instance.CmdSparky2(e.transform.position + e.GetComponent<SphereCollider>().center, hit.point, 0, null, null);
                    for (int i = 0; i < 5; i++)
                    {
                        if (t.layer == 8)
                        {
                            yeet = false;
                            i = 20;
                        }
                        else if (t.transform.parent != null) t = t.transform.parent.gameObject;

                    }
                }
                // if (yeet && (hit.collider.gameObject.layer == 8 && (hit.collider.transform.parent != null && (hit.collider.transform.parent.gameObject.layer == 8 || hit.collider.transform.parent.parent != null && (hit.collider.transform.parent.parent.gameObject.layer == 8 || hit.collider.transform.parent.parent.parent != null && hit.collider.transform.parent.parent.parent.gameObject.layer == 8)))))
                //     yeet = false;
                //if (yeet)
                //    Debug.Log("Touching Ground " + hit.collider);

                grounded = yeet;
                if (yeet)
                    return yeet;
            }
            else if (e.GetComponent<CapsuleCollider>() != null)
            {
                Effects.instance.CmdSparky2(e.transform.position + e.GetComponent<CapsuleCollider>().center, e.transform.position - Vector3.up * (0.25f * e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.1f), 0, null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<CapsuleCollider>().center, -Vector3.up, out hit, 0.25f*e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.01f, jumpMask);
                if (yeet)
                {
                    Effects.instance.CmdSparky2(e.transform.position + e.GetComponent<CapsuleCollider>().center, hit.point, 0, null, null);
                    GameObject t = hit.collider.gameObject;
                    for (int i = 0; i < 5; i++)
                    {
                        if (t.layer == 8)
                        {
                            yeet = false;
                            i = 20;
                        }
                        else if (t.transform.parent != null) t = t.transform.parent.gameObject;

                    }
                }
                //if (yeet && (gameObject.layer == 8 && (transform.parent != null && (transform.parent.gameObject.layer == 8 || transform.parent.parent != null && (transform.parent.parent.gameObject.layer == 8 || transform.parent.parent.parent != null && transform.parent.parent.parent.gameObject.layer == 8)))))
                //    yeet = false;
                //if (yeet)
                //    Debug.Log("Touching Ground");
                //if (yeet)
                //    Debug.Log("Touching Ground " + hit.collider);
                grounded = yeet;
                if (yeet)
                    return yeet;
            }
        //else if (e.GetComponent<SphereCollider>() != null)
        //    return Physics.Raycast(e.GetComponent<SphereCollider>().center, -Vector3.up, e.GetComponent<SphereCollider>().radius + 0.1f);
        grounded = false;
        return false;
    }

    private void OnCollisionStay(Collision collision)
    {
        /*if (canJump == false && collision.collider.tag == "Ground")
        {
            //canJump = true;
            //print("hit ground");
        }*/
    }
}
