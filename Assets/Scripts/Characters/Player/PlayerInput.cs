using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Animator))]
public sealed class PlayerInput : MonoBehaviour
{
    private PlayerMotor playerMotor;
    private float mouseSensitivity = 4f; //The player own settings for mouse sensitivity
    private bool isMenuOpen = false;
    private GameMenuManager gameMenuManager;
    private HudGUIManager hudGUIManager;
    private Animator animator;

    private RaycastHit hit;
    private Interactive interactive;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject defaultHitEffect;
    [SerializeField] private LayerMask attackLayer = 11;

    private bool attackReady = false;
    private bool attackReset = true;

    // Use this for initialization
    void Start ()
    {
        playerMotor = GetComponent<PlayerMotor>();
        mouseSensitivity = GameController.instance.MouseSensitivity;
        gameMenuManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<GameMenuManager>();
        GameController.OnMouseSensitivityUpdate += new GameController.MouseSensitivityUpdate(ChangeMouseSensitivity); //Subscribe to the mouse sensitivity
        ChangeMouseSensitivity(GameController.instance.MouseSensitivity); // Set the start value of the mouse sensitivity
        hudGUIManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<HudGUIManager>();
        animator = GetComponent<Animator>();
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
            isMenuOpen = !isMenuOpen;
            gameMenuManager.ToggleMenu(0, isMenuOpen);  
        }
        if (isMenuOpen)
            return;

        Vector3 moveHorizontal = transform.right * Input.GetAxisRaw("Horizontal");
        Vector3 moveVertical = transform.forward * Input.GetAxisRaw("Vertical");

        playerMotor.PerformMovement((moveHorizontal + moveVertical).normalized, Input.GetButton("Sprint"));
        playerMotor.PerformRotation(Input.GetAxisRaw("Mouse X") * mouseSensitivity, Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        if (Input.GetButtonDown("Jump"))
            playerMotor.PerformJump();

        if (Input.GetButtonDown("Holster"))
            playerMotor.CmdToogleHolster();

        if(Input.GetButtonDown("Attack"))
            playerMotor.CmdAttack();


        if (animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack") && attackReset)
        {
            attackReady = true;
            attackReset = false;
        }
        else if (!animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack") && !attackReset)
        {
            attackReset = true;
        }

        if (attackReady)
        {
            attackReady = false;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.9f, attackLayer))
            {
                Instantiate(defaultHitEffect).transform.position = hit.point;

                if (hit.transform.tag == "Player")
                    hit.transform.GetComponent<PlayerData>().CmdApplyDamage(10);

                else if (hit.transform.tag == "Interactable")
                    interactive.TakeDamage(transform.parent.gameObject, 10);
            }
        }

        else if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 1.9f))
        {
            if (hit.transform.tag == "Interactable")
            {
                try
                {
                    interactive = hit.transform.gameObject.GetComponent<Interactive>();
                    hudGUIManager.ShowInteractionText(true, interactive.GetName());

                    if (Input.GetButtonDown("Use"))
                        interactive.Activate(transform.parent.gameObject);
                }
                catch
                {
                    Debug.LogError("The object" + hit.transform.name + " has no interactive script placed on it");
                }
            }
        }
        else
            hudGUIManager.ShowInteractionText(false);



        
    }


    void OnDestroy()
    {
        GameController.OnMouseSensitivityUpdate -= new GameController.MouseSensitivityUpdate(ChangeMouseSensitivity);
    }
}
