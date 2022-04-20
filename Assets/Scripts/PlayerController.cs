
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
	
	void Update () {

        // If paused, return
        if (PauseGame.IsOn)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.Locked;

        // Calculate movement velocity as a 3D vector
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");
        Vector3 velocity = Vector3.zero;
        bool maxSpeed = false;
        float speed = walkSpeed;
        if (Mathf.Abs(xMov) + Mathf.Abs(zMov) > 0) {
            //Final Movement vector
            maxSpeed = true;
            if (!IsGrounded()) speed = flySpeed == 0 ? walkSpeed/2f : flySpeed;
            if (speed == flySpeed)
            {
                velocity += (motor.cam.transform.right * xMov + motor.cam.transform.forward * zMov).normalized * speed;
                //creature.flyAnim((Input.GetButton("Sprint") ? 2 : 1) * zMov / Mathf.Abs(zMov));
            }
            else
            {
                velocity += (transform.right * xMov + transform.forward * zMov).normalized * speed;
                //creature.walkAnim((Input.GetButton("Sprint") ? 2 : 1)*zMov/Mathf.Abs(zMov));
            }
            
            //Debug.Log("Max Speed = " + speed + ", rb.velocity = " + motor.rb.velocity.magnitude + ", velocity = " + velocity.magnitude + ", combined = " + (motor.rb.velocity + velocity).magnitude);

            /*
            if (motor.rb.velocity.x + velocity.x > speed) velocity.x = speed - motor.rb.velocity.x;
            if (motor.rb.velocity.x + velocity.x < -speed) velocity.x = -speed - motor.rb.velocity.x;
            if (motor.rb.velocity.z + velocity.z > speed) velocity.z = speed - motor.rb.velocity.z;
            if (motor.rb.velocity.z + velocity.z < -speed) velocity.z = -speed - motor.rb.velocity.z;*/
        } else
        {
            //if (creature != null)
                //creature.idleAnim();
        }
        //Apply movement
        //motor.Move(velocity);

        //Calculate rotation as a 3D vector (turning the player around)
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * mouseSensitivity;
        //Apply rotation
        motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vector (turning the camera)
        float xRot = Input.GetAxisRaw("Mouse Y");

        float camRotation = xRot * mouseSensitivity;
        //Apply rotation
        motor.CamRotate(camRotation);

        if (Input.GetButton("Jump") && (IsGrounded()))
        {
            if (maxSpeed)
                velocity /= 1 + Mathf.Max(0, (motor.rb.velocity + velocity).magnitude - speed) / speed;
            velocity.y = jumpForce;
        } else if ((Input.GetButton("Jump") || Input.GetButton("Crouch")) && flySpeed > 0)
        {
            maxSpeed = true;
            velocity += (Input.GetButton("Crouch") ? -1 : 1) * motor.cam.transform.up.normalized * flySpeed;
            velocity /= 1 + Mathf.Max(0, (motor.rb.velocity + velocity).magnitude - speed) / speed;
        }
        velocity *= Input.GetButton("Sprint") ? 2 : 1;
        motor.ApplyThruster(velocity*100);
    }

    bool IsGrounded() {
        RaycastHit hit;
        //int i = 0;
        foreach (GameObject e in GetComponent<Player>().chara.canJumpFrom)
            if (e.GetComponent<SphereCollider>() != null)
            {
                //Effects.instance.CmdSparky(e.transform.position+e.GetComponent<SphereCollider>().center, e.transform.position - Vector3.up*(e.GetComponent<SphereCollider>().radius + 0.1f), null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<SphereCollider>().center, -Vector3.up, out hit, e.GetComponent<SphereCollider>().radius*e.GetComponent<SphereCollider>().transform.localScale.magnitude + 0.1f, jumpMask);
                //if (yeet)
                //    Debug.Log("Touching Ground");
                if (yeet)
                    return yeet;
            }
            else if (e.GetComponent<CapsuleCollider>() != null)
            {
                //Effects.instance.CmdSparky(e.transform.position + e.GetComponent<CapsuleCollider>().center, e.transform.position - Vector3.up * (0.25f * e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.1f), null, null);
                bool yeet = Physics.Raycast(e.transform.position + e.GetComponent<CapsuleCollider>().center, -Vector3.up, out hit, 0.25f*e.GetComponent<CapsuleCollider>().height * e.GetComponent<CapsuleCollider>().transform.localScale.magnitude * chara.transform.localScale.magnitude + 0.1f, jumpMask);
                //if (yeet)
                //    Debug.Log("Touching Ground");
                if (yeet)
                    return yeet;
            }
            //else if (e.GetComponent<SphereCollider>() != null)
            //    return Physics.Raycast(e.GetComponent<SphereCollider>().center, -Vector3.up, e.GetComponent<SphereCollider>().radius + 0.1f);
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
