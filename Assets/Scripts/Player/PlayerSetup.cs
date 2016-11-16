using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{

    [SerializeField]
    Behaviour[] componentsToDisable;

    // Update is called once per frame
    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            Camera.main.gameObject.SetActive(false);

        }
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }


}
