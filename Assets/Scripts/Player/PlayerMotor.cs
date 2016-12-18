using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{
    private float currentCameraRotationX = 0.0f;
    private Rigidbody rb;
    private Vector3 currentDir;

    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private float sprintModifier = 2.0f;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;

    //Constants
    private const float MINIMUM_X = -90F;
    private const float MAXIMUM_X = 90F;
    private const float CAMERA_ROTATION_X_LIMIT = 75f;
    private const float JUMP_FORCE = 300;
    private const float WALK_ANIMATION_SYNC = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PerformJump()
    {
        if(Physics.Raycast(transform.position,Vector3.down, 1.2f))
            rb.AddForce(Vector3.up * JUMP_FORCE);
    }

    public void PerformMovement(Vector3 currentDir, bool isSprinting)
    {
        this.currentDir = currentDir;
        PlayerData.localPlayerInstance.SetPlayerSprint(isSprinting);
    }

    void FixedUpdate()
    {
        if (isLocalPlayer && currentDir != Vector3.zero)
        {
            if (PlayerData.localPlayerInstance.IsPlayerSprint())
                rb.AddForce(currentDir * (walkSpeed * sprintModifier), ForceMode.Acceleration);
            else
                rb.AddForce(currentDir * walkSpeed, ForceMode.Acceleration);
        }    
    }

    void Update()
    {
        animator.SetFloat("MovingSpeed", Mathf.Clamp(rb.velocity.magnitude/ WALK_ANIMATION_SYNC, 0 , 1));
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
