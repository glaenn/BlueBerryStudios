using UnityEngine;

public sealed class PlayerInteractor : MonoBehaviour
{
    private HudGUIManager hudGUIManager;
    private RaycastHit hit;
    private Interactive interactive;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject defaultHitEffect;
    [SerializeField] private LayerMask attackLayer = 11;

    private bool attackReady = false;
    private bool attackReset = true;

    void Awake()
    {
            hudGUIManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<HudGUIManager>();
    }

	// Update is called once per frame
	void Update ()
    {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, 1.9f))
            {
                if (hit.transform.tag == "Interactable")
                {
                    interactive = hit.transform.gameObject.GetComponent<Interactive>();
                    try
                    {
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
                        interactive.TakeDamage(transform.parent.gameObject,10);
                }
            }
        }
}