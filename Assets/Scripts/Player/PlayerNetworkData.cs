using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
/// <summary>
/// Holds are the player data.
/// Current Scene
/// Equipment and inventory
/// </summary>
public class PlayerNetworkData : NetworkBehaviour
{
    public static PlayerNetworkData localPlayerInstance = null;
    [SyncVar] private int currentScene = 2; //SyncVar makes sure that the server updates the variable to the clients
    //private GameController gameController;
    [SerializeField] private GameObject playerCharacter;
    private CapsuleCollider playerCollider;
    private Rigidbody rigidBody;

    // Use this for initialization
    void Start ()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();

        if (isLocalPlayer)
        {
            localPlayerInstance = this;
            ChangeScene(2); //Set start Scene
        }
    }
	
    //Locally changes the scene for the player
    public void ChangeScene(int scene)
    {
        for (int i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            SceneManager.UnloadScene(i);
        }

        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        CmdSetPlayerScene(scene);     
    }

    public void LateUpdate()
    {
        UpdateCharacterVisibility();
    }


    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetPlayerScene(int scene)
    {
        currentScene = scene;
        //RpcChangeScene(scene);
    }

    [ClientRpc] //This function will run on all Clients, including the one running the server
    void RpcChangeScene(int scene)
    {
        currentScene = scene;
    }

    private void UpdateCharacterVisibility()
    {
        if (currentScene != PlayerNetworkData.localPlayerInstance.currentScene)
        {
            rigidBody.useGravity = false;
            playerCharacter.SetActive(false);
            playerCollider.enabled = false;
        }
        else
        {
            rigidBody.useGravity = true;
            playerCharacter.SetActive(true);
            playerCollider.enabled = true;
        }
    }
}
