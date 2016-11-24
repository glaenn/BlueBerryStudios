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
    [SyncVar] private string currentScene = "Map01"; //SyncVar makes sure that the server updates the variable to the clients
    private HudGUIManager hudGUIManager;
    private string storedDestinationSpawnName;
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
            SceneManager.sceneLoaded += LoadSceneFinished;
            LoadScene(currentScene, "FirstSpawn"); //Set start Scene
            hudGUIManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<HudGUIManager>();
        }
    }
	
    //Locally changes the scene for the player
    public void LoadScene(string scene, string destinationSpawnName)
    {
        for (int i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            SceneManager.UnloadScene(i);
        }

        storedDestinationSpawnName = destinationSpawnName;

        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        CmdSetPlayerScene(scene);     
    }

    public void LoadSceneFinished(Scene scene, LoadSceneMode loadSceneMode)
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach(GameObject spawn in spawnPoints)
        {
            if(spawn.name == storedDestinationSpawnName)
            {
                Transform spawnPoint = spawn.transform;
                transform.position = new Vector3(spawnPoint.position.x + Random.Range(-spawnPoint.localScale.x / 2, spawnPoint.localScale.x / 2),
                                            spawnPoint.position.y,
                                            spawnPoint.position.z + Random.Range(-spawnPoint.localScale.z / 2, spawnPoint.localScale.z / 2));
                break;
            }
        }
    }

    public void LateUpdate()
    {
        UpdateCharacterVisibility();
    }


    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetPlayerScene(string scene)
    {
        currentScene = scene;
    }

    [ClientRpc] //This function will run on all Clients, including the one running the server
    void RpcChangeScene(string scene)
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
