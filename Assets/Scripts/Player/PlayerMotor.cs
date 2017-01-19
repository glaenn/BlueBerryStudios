using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{
    private Rigidbody rb;
    private Vector3 currentDir;
    private List<Vector3> smoothedVelocity = new List<Vector3>();
    private bool isGrounded;

    [SyncVar]
    private bool isHolstered = true;
    [SyncVar]
    private float currentCameraRotationX = 0.0f;

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
    private const float WALK_ANIMATION_SYNC = 6.5f;
    private const float MOVEMENT_DRAG = 0.95f;
    private const float SPINE_ROT = 20.0f;

    private List<float> rotArrayX = new List<float>();
    private List<float> rotArrayY = new List<float>();
    [SerializeField] private float frameCounter = 20.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator.SetBool("isHolstered", isHolstered);
    }

    public void PerformJump()
    {
        if(isGrounded)
            rb.AddForce(Vector3.up * JUMP_FORCE);
    }

    public void PerformMovement(Vector3 currentDir, bool isSprinting)
    {
        this.currentDir = currentDir;
        PlayerData.localPlayerInstance.SetPlayerSprint(isSprinting);
    }

    void FixedUpdate()
    {
        if (isLocalPlayer && currentDir != Vector3.zero && isGrounded)
        {
            if (PlayerData.localPlayerInstance.IsPlayerSprinting() && rb.velocity.magnitude < maxVelocity * sprintModifier)
                rb.AddForce(currentDir * (velocityMultiplier * sprintModifier), ForceMode.Acceleration);
            else if (rb.velocity.magnitude < maxVelocity)
                rb.AddForce(currentDir * velocityMultiplier, ForceMode.Acceleration);
        }

        //Movement drag on the ground. Doesn't affect vertical movement
        if (isLocalPlayer && isGrounded)
            rb.velocity = new Vector3(rb.velocity.x * MOVEMENT_DRAG, rb.velocity.y, rb.velocity.z * MOVEMENT_DRAG);

        smoothedVelocity.Add(transform.InverseTransformDirection(rb.velocity));

        if (smoothedVelocity.Count >= 5)
            smoothedVelocity.RemoveAt(0);

        Vector3 calculatedMovingSpeed = Vector3.zero;

        for (int i = 0; i < smoothedVelocity.Count; i++)
            calculatedMovingSpeed += smoothedVelocity[i];

        calculatedMovingSpeed /= smoothedVelocity.Count;

        animator.SetFloat("movingSpeed", Mathf.Clamp((Mathf.Abs(calculatedMovingSpeed.x) + Mathf.Abs(calculatedMovingSpeed.z)) / WALK_ANIMATION_SYNC, 0, 1));

        if (!isGrounded)
            animator.SetBool("isInAir", true);
        else
            animator.SetBool("isInAir", false);
    }

    void LateUpdate()
    {
        if (isLocalPlayer)
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, 0.80f + currentCameraRotationX / 500, currentCameraRotationX / 200);
       
        spine.Rotate(currentCameraRotationX / 2f, 0, 0);
        clavice_l.Rotate(0, -currentCameraRotationX / 3.5f, 0);
        clavice_r.Rotate(0, currentCameraRotationX / 3.5f, 0);
        neck.Rotate(currentCameraRotationX / 2, 0, 0);

    if (Physics.Raycast(transform.position, Vector3.down, 1.2f))
        isGrounded = true;
    else
        isGrounded = false;

    }
    [Command] //This function will run on the server when it is called on the client.
    public void CmdToogleHolster()
    {
        isHolstered = !isHolstered;
        RpcToogleHolster(isHolstered);
    }

    [ClientRpc] //This fuction will run on all clients when called from the server
    public void RpcToogleHolster(bool isHolstered)
    {
        animator.SetBool("isHolstered", isHolstered);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdAttack()
    {
        if (isHolstered)
        {
            isHolstered = false;
            RpcToogleHolster(isHolstered);
        }
        RpcAttack(); 
    }

    [ClientRpc] //This fuction will run on all clients when called from the server
    public void RpcAttack()
    {
        animator.SetTrigger("attack");

        if(isLocalPlayer)
        {
            //Check if we hit something with the attack
            if (Physics.Raycast(transform.position, Vector3.forward, 1.2f))
            {
               
            }
        }

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
        CmdSetCameraRotation(currentCameraRotationX);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetCameraRotation(float currentCameraRotationX)
    {
        this.currentCameraRotationX = currentCameraRotationX;
    }
}
