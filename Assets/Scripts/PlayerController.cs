
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
    private float jumpForce = 5f;

    //private bool canJump = true;
    private bool canFly = false;

    private PlayerMotor motor;

    private Character chara;

	void Start () {
        motor = GetComponent<PlayerMotor>();
	}

    public void Setup()
    {
        chara = GetComponent<Player>().chara;
        walkSpeed = (chara.WALK_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult;
        climbSpeed = (chara.CLIMB_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult;
        flySpeed = (chara.FLY_SPEED * 2.4533f / 81f) * GameManager.instance.matchSettings.speedMult;
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
        if (Mathf.Abs(xMov) + Mathf.Abs(zMov) > 0) {
            //Final Movement vector
            float speed = Mathf.Max(walkSpeed,flySpeed);
            if (!IsGrounded()) speed = flySpeed == 0 ? walkSpeed/2 : flySpeed;
            if (speed == flySpeed)
                velocity += (motor.cam.transform.right * xMov + motor.cam.transform.forward * zMov).normalized * speed;
            else
                velocity += (transform.right * xMov + transform.forward * zMov).normalized * speed;
            //Debug.Log("Max Speed = " + speed + ", rb.velocity = " + motor.rb.velocity.magnitude + ", velocity = " + velocity.magnitude + ", combined = " + (motor.rb.velocity + velocity).magnitude);

            velocity /= 1 + Mathf.Max(0, (motor.rb.velocity + velocity).magnitude - speed) / speed;
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
        motor.Rotate(rotation);

        //Calculate camera rotation as a 3D vector (turning the camera)
        float xRot = Input.GetAxisRaw("Mouse Y");

        float camRotation = xRot * mouseSensitivity;
        //Apply rotation
        motor.CamRotate(camRotation);

        if (Input.GetButton("Jump") && (canFly || motor.canJump()))
        {
            velocity.y = jumpForce;
        }

        motor.ApplyThruster(velocity*100);
    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, chara.distanceToGround + 0.1f);
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
