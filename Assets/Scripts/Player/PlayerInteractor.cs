using UnityEngine;
using UnityEngine.Networking;

public class PlayerInteractor : NetworkBehaviour
{
    private HudGUIManager hudGUIManager;
    private RaycastHit hit;
    private Interactive interactive;
    private int layerMask = 1 << 8;

    void Awake()
    {
        hudGUIManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<HudGUIManager>();
    }

	// Update is called once per frame
	void Update ()
    {
        if(Physics.Raycast(transform.position, transform.forward,out hit, 1.9f))
        {
            if (hit.transform.tag == "Interactable")
            {
                interactive = hit.transform.gameObject.GetComponent<Interactive>();

                try
                {
                    hudGUIManager.ShowInteractionText(interactive.GetName());

                    if (Input.GetButtonDown("Use"))
                    {
                        interactive.Activate(transform.parent.gameObject);
                    }
                }
                catch
                {
                    Debug.LogError("The object" + hit.transform.name + " has no interactive script placed on it");
                }
            }
        }
        else
            hudGUIManager.HideInteractionText();

    }
}
