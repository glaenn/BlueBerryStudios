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
            SceneManager.sceneLoaded += LoadSceneFinished;
            LoadScene(currentScene); //Set start Scene
        }
    }
	
    //Locally changes the scene for the player
    public void LoadScene(int scene)
    {
        for (int i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            SceneManager.UnloadScene(i);
        }

        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        CmdSetPlayerScene(scene);     
    }

    public void LoadSceneFinished(Scene scene, LoadSceneMode loadSceneMode)
    {
        Transform spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        
        transform.position = new Vector3(   spawnPoint.position.x + Random.Range(-spawnPoint.localScale.x / 2, spawnPoint.localScale.x / 2),
                                            spawnPoint.position.y,
                                            spawnPoint.position.z + Random.Range(-spawnPoint.localScale.z / 2, spawnPoint.localScale.z / 2));

        transform.rotation = spawnPoint.rotation;

        Debug.Log(transform.position);

        /*
        try
        {
            foreach (GameObject point in spawnPoint)
            {


            }
        }
        catch
        {
            Debug.LogError("There is no spawn point in " + scene.name);
        }
        */

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
