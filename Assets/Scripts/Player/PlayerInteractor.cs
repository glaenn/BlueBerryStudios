using UnityEngine;
using System;

public class PlayerInteractor : MonoBehaviour
{
    private RaycastHit hit;
    private Interactive new_interactive;
    private int layerMask = 1 << 8;


    void Awake()
    {

    }

	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if(Physics.Raycast(transform.position, transform.forward,out hit, 10.0f, layerMask))
        { 
            try
            {
                new_interactive = hit.transform.gameObject.GetComponent<Interactive>();
                Debug.Log(new_interactive.GetName());

            }
            catch
            {
                Debug.LogError("The object" + hit.transform.name +" has no interactive script placed on it");
            }

            if (Input.GetButtonDown("Use"))
            {
                new_interactive.Use();
            }

        }

	}
}
