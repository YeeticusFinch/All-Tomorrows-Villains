using System.Collections;
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
    public float cameraRotationLimit = 85f;
    private float currentCameraRotation = 0f;

    private Vector3 acceleration = Vector3.zero;
    private Vector3 prevVel = Vector3.zero;

    private Vector3 initialRot;
    private Vector3 flyRot;

    [SerializeField] GameObject player;

    public float roll = 0f;
    public float rollTarget = 0f;

    private bool flying;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        initialRot = transform.eulerAngles;
        flyRot = initialRot;
    }

    public void Move(Vector3 vel)
    {
        //vel.y += rb.velocity.y;
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
        rollTarget = -Mathf.Clamp(Vector3.Dot(f, cam.transform.right)/20f,-90f, 90f);
        //if (rollTarget > 0)
        //    Debug.Log("rollTarget = " + rollTarget);
    }

    public void Jump(Vector3 f)
    {
        int n = 5;
        int delay = 10;
        Vector3 step = (f.normalized*2000+0.1f*f) / n;
        if (!isJumping)
            StartCoroutine(ForceInstallments(step, n, delay));
        //force = f;
        //rollTarget = -Mathf.Clamp(Vector3.Dot(f, cam.transform.right) / 20f, -90f, 90f);
        //if (rollTarget > 0)
        //    Debug.Log("rollTarget = " + rollTarget);
    }

    public Vector3 getVelocity()
    {
        return new Vector3(rb.velocity.x, Mathf.Max(0, rb.velocity.y), rb.velocity.z);
    }

    public Vector3 getHorizontalVelocity()
    {
        return new Vector3(rb.velocity.x, 0, rb.velocity.z);
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
            GetComponent<Player>().CmdTakeDamage(acceleration.magnitude*60f/(GameManager.instance.matchSettings.moveSpeedMult), "bludgeoning", 12, "dexterity");
        }
        //if (fi % 5 == 0 && GetComponent<Player>().chara != null && GetComponent<Player>().model != null && (GetComponent<Player>().chara.rotateWithCamera || (GetComponent<Player>().chara.rotateWithCameraWhenFlying && !GetComponent<Player>().IsGrounded())))
        if (fi % 5 == 0 && GetComponent<Player>().chara != null && GetComponent<Player>().model != null)
            GetComponent<Player>().CmdRotate(GetComponent<Player>().model.transform.eulerAngles);
        fi++;
        fi %= 10000;
    }   

    void PerformMovement()
    {
        if (roll < rollTarget)
            roll += Mathf.Min(rollTarget - roll, 1.5f);
        if (roll > rollTarget)
            roll += Mathf.Max(rollTarget - roll, -1.5f);

        /*if (cam.transform.eulerAngles.z < rollTarget)
            cam.transform.eulerAngles += cam.transform.forward * Mathf.Min(rollTarget - cam.transform.eulerAngles.z, 0.2f);
        if (cam.transform.eulerAngles.z > rollTarget)
            cam.transform.eulerAngles += cam.transform.forward * Mathf.Max(rollTarget - cam.transform.eulerAngles.z, -0.2f);
*/
        if (velocity != Vector3.zero)
        {
            //rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            //if (Mathf.Abs(velocity.y) < 0.1f )
            //    rb.velocity += CarlMath.compMult(MinV(velocity - rb.velocity, 0.2f * (velocity - rb.velocity).normalized), 1, 0, 1);
            //else
                rb.velocity += MinV(velocity + rb.velocity.y * Vector3.up - rb.velocity, 0.2f*(velocity + rb.velocity.y * Vector3.up - rb.velocity).normalized);
            //rb.velocity = velocity;
            //Debug.Log("Velocity = " + rb.velocity + " Target Velocity = " + velocity);
            velocity = MaxV(velocity - 0.1f * velocity.normalized, Vector3.zero);
            if (velocity.magnitude < 0.1f)
                velocity = Vector3.zero;
        }
        if (force + jumpForce != Vector3.zero)
        {
            rb.AddForce((force+jumpForce)*Time.fixedDeltaTime, ForceMode.Acceleration);
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
        //player.transform.localRotation = Quaternion.Euler(player.transform.rotation.eulerAngles.x, player.transform.rotation.eulerAngles.y, roll);

        if (cam != null)
        {
            float prevCamRotation = currentCameraRotation;
            currentCameraRotation -= camRotation;
            currentCameraRotation = Mathf.Clamp(currentCameraRotation, -cameraRotationLimit, cameraRotationLimit); // Clamp rotation

            //bool doubleRoll = GetComponent<Player>().chara != null && GetComponent<Player>().chara.camAttachTo != null;

            cam.transform.localEulerAngles = new Vector3(currentCameraRotation, 0, roll); // Apply rotation to camera

            if (GetComponent<Player>().chara != null)
            {
                bool crossRotate = GetComponent<Player>().charId >= 1 && GetComponent<Player>().charId <= 3;
                if (crossRotate)
                    flyRot += Vector3.Cross(new Vector3((currentCameraRotation - prevCamRotation) * GetComponent<Player>().chara.rotateWithCamScalars.x, 0f, 0f), GetComponent<Player>().chara.rotate.normalized);
                else
                    flyRot -= new Vector3((currentCameraRotation - prevCamRotation) * GetComponent<Player>().chara.rotateWithCamScalars.x, 0, 0);
            }
            if (GetComponent<Player>().chara != null && GetComponent<Player>().model != null && (GetComponent<Player>().chara.rotateWithCamera || (GetComponent<Player>().chara.rotateWithCameraWhenFlying && !GetComponent<Player>().IsGrounded())))
            {
                //GetComponent<Player>().CmdRotate(Vector3.Cross(new Vector3(currentCameraRotation - prevCamRotation, 0f, 0f), GetComponent<Player>().chara.rotate.normalized));
                
                if (GetComponent<Player>().chara.lerpRotate)
                {
                    float rotX = CarlMath.angleFix(GetComponent<Player>().model.transform.eulerAngles.x);
                    float rotZ = CarlMath.angleFix(GetComponent<Player>().model.transform.eulerAngles.z);
                    GetComponent<Player>().model.transform.eulerAngles = new Vector3(rotX + CarlMath.MinMag((flyRot.x - rotX), 2 * Mathf.Sign(flyRot.x - rotX)/* / Mathf.Abs(flyRot.x - rotX)*/), GetComponent<Player>().model.transform.eulerAngles.y, flyRot.z)/* + (crossRotate ? Vector3.Cross(Vector3.forward*roll, GetComponent<Player>().chara.rotate.normalized) : -Vector3.forward*roll)*/;
                }
                else
                    GetComponent<Player>().model.transform.eulerAngles = new Vector3(flyRot.x, GetComponent<Player>().model.transform.eulerAngles.y, flyRot.z)/* + (crossRotate ? Vector3.Cross(Vector3.forward*roll, GetComponent<Player>().chara.rotate.normalized) : -Vector3.forward*roll)*/;
                flying = true;

            } else if (GetComponent<Player>().chara != null && GetComponent<Player>().chara.rotateWithCameraWhenFlying && GetComponent<Player>().IsGrounded())
            {
                //Debug.Log("rot shit: " + Mathf.Round(initialRot.x) + " - " + Mathf.Round(GetComponent<Player>().model.transform.eulerAngles.x));
                
                if (GetComponent<Player>().chara.lerpRotate)
                {
                    float rotX = CarlMath.angleFix(GetComponent<Player>().model.transform.eulerAngles.x);
                    float rotZ = CarlMath.angleFix(GetComponent<Player>().model.transform.eulerAngles.z);
                    GetComponent<Player>().model.transform.eulerAngles += new Vector3(CarlMath.MinMag((initialRot.x - rotX), 2 * Mathf.Sign(initialRot.x - rotX)), 0/*GetComponent<Player>().model.transform.eulerAngles.y*/, CarlMath.MinMag(initialRot.z - rotZ, 2 * Mathf.Sign(initialRot.z - rotZ)))/* + (crossRotate ? Vector3.Cross(Vector3.forward * roll, GetComponent<Player>().chara.rotate.normalized) : -Vector3.forward * roll)*/;
                }
                else
                    GetComponent<Player>().model.transform.eulerAngles = new Vector3(initialRot.x, GetComponent<Player>().model.transform.eulerAngles.y, initialRot.z)/* + (crossRotate ? Vector3.Cross(Vector3.forward * roll, GetComponent<Player>().chara.rotate.normalized) : -Vector3.forward * roll)*/;
                flying = false;
            }
        }
        
    }
        
    public bool canJump()
    {
        if (Mathf.Abs(acceleration.y) > 0.001)
            return false;
        return true;
    }

    public bool isJumping = false;
    public Vector3 jumpForce = Vector3.zero;

    IEnumerator ForceInstallments(Vector3 step, int n, int delay)
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpForce = Vector3.zero;
            for (int i = 0; i < n; i++)
            {
                jumpForce += step;
                yield return new WaitForSecondsRealtime(delay/1000f);
            }
            for (int i = 0; i < n; i++)
            {
                jumpForce -= step;
                yield return new WaitForSecondsRealtime(delay / 1000f);
            }
            jumpForce = Vector3.zero;
            isJumping = false;
        }
    }
}
