using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerSceneManager : NetworkBehaviour
{
    private CapsuleCollider playerCollider;
    private Rigidbody rigidBody;
    [SerializeField] private GameObject playerCharacter;
    PlayerNetworkData playerNetworkData;

    private string destinationName;

    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();
        playerNetworkData = GetComponent<PlayerNetworkData>();

        if (isLocalPlayer)
        {
        SceneManager.sceneLoaded += LoadSceneFinished;
        }

    }

    //Locally changes the scene for the player
    public void LoadScene(string scene, string destinationName)
    {
        //Unloads all the previous scenes
        for (int i = 2; i < SceneManager.sceneCountInBuildSettings; i++)
            SceneManager.UnloadScene(i);

        //Stores the destination for this destination 
        this.destinationName = destinationName;

        //Begin loading scen
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        
        //Sends the scene change to server
        playerNetworkData.CmdSetPlayerScene(scene);
    }

    public void LoadSceneFinished(Scene scene, LoadSceneMode loadSceneMode)
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject spawn in spawnPoints)
        {
            if (spawn.name == destinationName)
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


    private void UpdateCharacterVisibility()
    {
        if (playerNetworkData.GetPlayerScene() != PlayerNetworkData.localPlayerInstance.GetPlayerScene())
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
