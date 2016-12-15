using UnityEngine.Networking;
using UnityEngine;

public class NetworkSpawner : NetworkBehaviour
{
    [SerializeField] GameObject serverController;

    public override void OnStartServer()
    {
        Debug.Log("spawn cube");
        NetworkServer.Spawn(Instantiate(serverController));
    }
}
