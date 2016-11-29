using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerInput))]

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] GameObject playerCamera;

    // Only called on localplyrt
    public override void OnStartLocalPlayer()
    {
        GetComponent<PlayerInput>().enabled = true;
        playerCamera.SetActive(true);
        base.OnStartLocalPlayer();
    }

}
