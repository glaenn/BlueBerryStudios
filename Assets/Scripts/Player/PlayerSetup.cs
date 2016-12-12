using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerInput))]

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] GameObject playerEyes;
    [SerializeField] GameObject playerHair;
    [SerializeField] GameObject playerFace;


    // Only called on localplayer
    public override void OnStartLocalPlayer()
    {
        GetComponent<PlayerInput>().enabled = true;
        playerCamera.SetActive(true);
        playerEyes.SetActive(false);
        playerHair.SetActive(false);
        playerFace.SetActive(false);


    }

}
