using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerInput))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField] GameObject playerCamera;
    [SerializeField] SkinnedMeshRenderer playerEyes;
    [SerializeField] SkinnedMeshRenderer playerHair;
    [SerializeField] SkinnedMeshRenderer playerFace;
    [SerializeField] SkinnedMeshRenderer playerHands;

    // Only called on localplayer
    public override void OnStartLocalPlayer()
    {
        GetComponent<PlayerInput>().enabled = true;
        playerCamera.SetActive(true);
        playerEyes.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        playerHair.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        playerFace.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        playerHands.gameObject.layer = 10;
        gameObject.layer = 11;
    }

}
