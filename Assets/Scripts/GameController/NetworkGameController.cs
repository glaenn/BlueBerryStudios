using UnityEngine.Networking;
using UnityEngine;

public class NetworkGameController : NetworkBehaviour
{
    [SerializeField] GameObject networkGameController;

    public override void OnStartServer()
    {
        NetworkServer.Spawn(Instantiate(networkGameController));
    }

}
