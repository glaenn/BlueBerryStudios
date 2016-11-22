using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Vector3 velocity;
    private float characterRotation;
    private float cameraRotationX = 0.0f;
    private float currentCameraRotationX = 0.0f;
    private Rigidbody rb;

    [SerializeField] private float speed = 5.0f;

    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;

    //Constants
    private const float MINIMUM_X = -90F;
    private const float MAXIMUM_X = 90F;
    private const float CAMERA_ROTATION_X_LIMIT = 85f;
    private const float JUMPFORCE = 300;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity*speed;
    }

    public void SetCharacterRotation(float characterRotation)
    {
        this.characterRotation = characterRotation;
    }

    public void SetCameraRotation(float cameraRotationX)
    {
        this.cameraRotationX = cameraRotationX;
    }

    public void PerformJump()
    {
        if(Physics.Raycast(transform.position,Vector3.down, 1.0f))
            rb.AddForce(Vector3.up * JUMPFORCE);
    }

    void Update()
    {
        PerformMovement();
        PerformRotation();  
    }
	
    private void PerformMovement()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.deltaTime);
            animator.SetFloat("MovingSpeed", 1);
        }
        else
        {
            animator.SetFloat("MovingSpeed", 0);
        }
    }

    private void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0.0f, characterRotation, 0.0f)));

        if (cam != null)
        {
            // Set our rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -CAMERA_ROTATION_X_LIMIT, CAMERA_ROTATION_X_LIMIT);

            //Apply our rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}
