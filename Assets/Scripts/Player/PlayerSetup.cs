using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerLocalController))]

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] GameObject playerCamera;

    // Only called on localplyrt
    public override void OnStartLocalPlayer()
    {
        GetComponent<PlayerLocalController>().enabled = true;
        playerCamera.SetActive(true);
        base.OnStartLocalPlayer();
    }

    void Start()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
    }

}
