using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{
    private float currentCameraRotationX = 0.0f;
    private Rigidbody rb;
    private Vector3 currentDir;

    [SerializeField] private float velocityMultiplier = 15.0f;
    [SerializeField] private float maxVelocity = 10.0f;
    [SerializeField] private float sprintModifier = 2.0f;
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform spine;
    [SerializeField] private Transform clavice_l;
    [SerializeField] private Transform clavice_r;
    [SerializeField] private Transform neck;

    //Constants
    private const float CAMERA_ROTATION_X_LIMIT = 45f;
    private const float JUMP_FORCE = 500;
    private const float WALK_ANIMATION_SYNC = 3;
    private const float MOVEMENT_DRAG = 0.95f;
    private const float SPINE_ROT = 20.0f;

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
                if (PlayerData.localPlayerInstance.IsPlayerSprint() && rb.velocity.magnitude < maxVelocity*sprintModifier)
                    rb.AddForce(currentDir * (velocityMultiplier * sprintModifier), ForceMode.Acceleration);
                else if (rb.velocity.magnitude < maxVelocity)
                    rb.AddForce(currentDir * velocityMultiplier, ForceMode.Acceleration);
        }

        //Created rigidbody drag that doesn't affect falling down
        rb.velocity = new Vector3(rb.velocity.x * MOVEMENT_DRAG, rb.velocity.y, rb.velocity.z * MOVEMENT_DRAG);
          
    }

    void LateUpdate()
    {
        animator.SetFloat("MovingSpeed", Mathf.Clamp(rb.velocity.magnitude/ WALK_ANIMATION_SYNC, 0 , 1));

        if(isLocalPlayer)
        {
            //cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, Mathf.Clamp(0.18f, 0.18f + (transform.InverseTransformDirection(rb.velocity).z/25)));
        }

        /*
        float bodyRotation = currentCameraRotationX;
        spine.Rotate( Mathf.Clamp(bodyRotation, -SPINE_ROT, SPINE_ROT),0, 0);

        if (bodyRotation > SPINE_ROT || bodyRotation < -SPINE_ROT)
        { 
            if (currentCameraRotationX > SPINE_ROT)
                bodyRotation -= SPINE_ROT;
            else if (currentCameraRotationX < -SPINE_ROT)
                bodyRotation += SPINE_ROT;

            clavice_l.Rotate(0, -bodyRotation, 0);
            clavice_r.Rotate(0, bodyRotation, 0);
            neck.Rotate(bodyRotation, 0, 0);
        }
        */
        spine.Rotate(currentCameraRotationX / 2, 0, 0);
        clavice_l.Rotate(0, -currentCameraRotationX/4, 0);
        clavice_r.Rotate(0, currentCameraRotationX / 4, 0);
        neck.Rotate(currentCameraRotationX / 2, 0, 0);
    }

    public void ToogleHolster()
    {
        animator.SetBool("Holster", !animator.GetBool("Holster"));
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
