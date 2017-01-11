using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public sealed class PlayerInput : MonoBehaviour
{
    private PlayerMotor playerMotor;
    private float mouseSensitivity = 2.5f; //The player own settings for mouse sensitivity
    private bool isInMeny = false;
    GameMenuManager gameMenuManager;

    // Use this for initialization
    void Start ()
    {
        playerMotor = GetComponent<PlayerMotor>();
        mouseSensitivity = GameController.instance.MouseSensitivity;
        gameMenuManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<GameMenuManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetButtonDown("GameMeny"))
        {
            isInMeny = !isInMeny;

            if (isInMeny)
                gameMenuManager.OpenMeny(0);
            else
                gameMenuManager.CloseMeny();

        }
        if (isInMeny)
            return;

        Vector3 moveHorizontal = transform.right * Input.GetAxisRaw("Horizontal");
        Vector3 moveVertical = transform.forward * Input.GetAxisRaw("Vertical");

        playerMotor.PerformMovement((moveHorizontal + moveVertical).normalized, Input.GetButton("Sprint"));
        playerMotor.PerformRotation(Input.GetAxisRaw("Mouse X") * mouseSensitivity, Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        //Jump
        if (Input.GetButtonDown("Jump"))
        {
            playerMotor.PerformJump();
        }

        //Holter
        if (Input.GetButtonDown("Holster"))
        {
            playerMotor.ToogleHolster(); 
        }

    }
}
