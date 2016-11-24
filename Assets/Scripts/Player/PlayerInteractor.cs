using UnityEngine;
using System;

public class PlayerInteractor : MonoBehaviour
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
        if(Physics.Raycast(transform.position, transform.forward,out hit, 3.0f, layerMask))
        {
            interactive = hit.transform.gameObject.GetComponent<Interactive>();

            try
            {
                hudGUIManager.ShowInteractionText(interactive.GetName());

                if (Input.GetButtonDown("Use"))
                {
                    interactive.Use(transform.parent.gameObject);
                }
            }
            catch
            {
                Debug.LogError("The object" + hit.transform.name +" has no interactive script placed on it");
            }
        }
        else
            hudGUIManager.HideInteractionText();

    }
}
