using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{
    private float currentCameraRotationX = 0.0f;
    private Rigidbody rb;
    private Vector3 currentDir;

    [SerializeField] private float walkSpeed = 18.0f;
    [SerializeField] private float sprintModifier = 2.0f;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;

    //Constants
    private const float MINIMUM_X = -90F;
    private const float MAXIMUM_X = 90F;
    private const float CAMERA_ROTATION_X_LIMIT = 75f;
    private const float JUMP_FORCE = 300;
    private const float WALK_ANIMATION_SYNC = 3;

    private List<float> rotArrayX = new List<float>();
    private List<float> rotArrayY = new List<float>();
    [SerializeField] private float frameCounter = 20.0f;

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

    public void PerformRotation(float characterRotationY, float cameraRotationX)
    {
        float rotAverageY = 0f;
        float rotAverageX = 0f;

        rotArrayX.Add(cameraRotationX);
        rotArrayY.Add(characterRotationY);

        if (rotArrayY.Count >= frameCounter)
            rotArrayY.RemoveAt(0);

        if (rotArrayX.Count >= frameCounter)
            rotArrayX.RemoveAt(0);

        for (int j = 0; j < rotArrayY.Count; j++)
            rotAverageY += rotArrayY[j];

        for (int i = 0; i < rotArrayX.Count; i++)
            rotAverageX += rotArrayX[i];

        rotAverageY /= rotArrayY.Count;
        rotAverageX /= rotArrayX.Count;

        rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(rotAverageY, Vector3.up));

        if (cam != null)
        {
            // Set our rotation and clamp it
            currentCameraRotationX -= rotAverageX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -CAMERA_ROTATION_X_LIMIT, CAMERA_ROTATION_X_LIMIT);

            //Apply our rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);

        }

      
    }
}
