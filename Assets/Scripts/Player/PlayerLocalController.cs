﻿using UnityEngine;

public class PlayerLocalController : MonoBehaviour
{
    private PlayerMotor playerMotor;
    private PlayerNetworkData playerNetworkData;
    private float mouseSensitivity = 1.0f; //The player own settings for mouse sensitivity

    // Use this for initialization
    void Start ()
    {
        playerMotor = GetComponent<PlayerMotor>();
        playerNetworkData = GetComponent<PlayerNetworkData>(); 
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Calculate movement as 3D vector
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMove;
        Vector3 moveVertical = transform.forward * zMove;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized;

        playerMotor.PerformMovement(velocity);

        //Calculate character rotation (turning around)
        float yRot = Input.GetAxisRaw("Mouse X") * mouseSensitivity;

        //Calculate camera rotation (looking up and down)
        float xRot = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
   
        playerMotor.PerformRotation(yRot, xRot);

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            playerMotor.PerformJump();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
            playerNetworkData.LoadScene(2);
        else if (Input.GetKeyDown(KeyCode.Keypad2))
            playerNetworkData.LoadScene(3);

    }

 
}
