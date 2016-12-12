using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{
    private float currentCameraRotationX = 0.0f;
    private Rigidbody rb;
    [SyncVar]private Vector3 velocity;

    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float currentRunSpeed = 0.0f;

    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;

    //Constants
    private const float MINIMUM_X = -90F;
    private const float MAXIMUM_X = 90F;
    private const float CAMERA_ROTATION_X_LIMIT = 75f;
    private const float JUMPFORCE = 300;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PerformJump()
    {
        if(Physics.Raycast(transform.position,Vector3.down, 1.2f))
            rb.AddForce(Vector3.up * JUMPFORCE);
    }

    public void PerformMovement(Vector3 velocity)
    {
        CmdSetVelocity(velocity);

        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * speed * Time.deltaTime);
            
        }
    }


    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;

    }

    void Update()
    {
        if (velocity != Vector3.zero)
        {
            currentRunSpeed += Time.deltaTime * 3;
        }
        else
        {
            currentRunSpeed -= Time.deltaTime * 3;
        }
 
        currentRunSpeed = Mathf.Clamp(currentRunSpeed, 0, 1);
        animator.SetFloat("MovingSpeed", currentRunSpeed);
    }

    public void PerformRotation(float characterRotation, float cameraRotationX)
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0.0f, characterRotation, 0.0f)));

        if (cam != null)
        {
            // Set our rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -CAMERA_ROTATION_X_LIMIT, CAMERA_ROTATION_X_LIMIT);

            //Apply our rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

            //cam.transform.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
            //        smoothTime * Time.deltaTime);

        }
    }
}
