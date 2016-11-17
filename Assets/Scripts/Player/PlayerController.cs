using System;
using UnityEngine;
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private float mouseSensitivity = 1.0f;

    private PlayerMotor playerMotor;

    void Start()
    {
        playerMotor = GetComponent<PlayerMotor>();
    }
	
    void Update()
    {
        //Calculate movement as 3D vector
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        playerMotor.SetVelocity(velocity);

        //Calculate character rotation (turning around)
        float yRot = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        playerMotor.SetCharacterRotation(yRot);

        //Calculate camera rotation (looking up and down)
        float xRot = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        playerMotor.SetCameraRotation(xRot);

        //Jump
        if(Input.GetButtonDown("Jump"))
        {
            playerMotor.PerformJump();
        }
    }



}
