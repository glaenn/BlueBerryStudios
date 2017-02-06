using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public sealed class PlayerInput : MonoBehaviour
{
    private PlayerMotor playerMotor;
    private float mouseSensitivity = 4f; //The player own settings for mouse sensitivity
    private bool isInMeny = false;
    GameMenuManager gameMenuManager;

    // Use this for initialization
    void Start ()
    {
        playerMotor = GetComponent<PlayerMotor>();
        mouseSensitivity = GameController.instance.MouseSensitivity;
        gameMenuManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<GameMenuManager>();
        GameController.OnMouseSensitivityUpdate += new GameController.MouseSensitivityUpdate(ChangeMouseSensitivity); //Subscribe to the mouse sensitivity
        ChangeMouseSensitivity(GameController.instance.MouseSensitivity); // Set the start value of the mouse sensitivity
    }

    void ChangeMouseSensitivity(float sensitivity)
    {
        mouseSensitivity = sensitivity;
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

        if (Input.GetButtonDown("Jump"))
        {
            playerMotor.PerformJump();
        }

        if (Input.GetButtonDown("Holster"))
        {
            playerMotor.CmdToogleHolster();
        }

        if(Input.GetButtonDown("Attack"))
        {
            playerMotor.CmdAttack();
        }
    }

    void OnDestroy()
    {
        GameController.OnMouseSensitivityUpdate -= new GameController.MouseSensitivityUpdate(ChangeMouseSensitivity);
    }
}
