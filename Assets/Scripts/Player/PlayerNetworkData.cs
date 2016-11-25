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
    private HudGUIManager hudGUIManager;

    //Player variables
    [SyncVar] private string playerScene = "Map01"; //SyncVar makes sure that the server updates the variable to the clients
    [SyncVar] private int playerMaxHealth = 100;
    [SyncVar] private int playerCurrentHealth = 100;
    private string resurrectionSpawnPoint;

    // Use this for initialization
    void Start ()
    {
        if (isLocalPlayer)
        {
            localPlayerInstance = this;
            hudGUIManager = GameObject.FindGameObjectWithTag("InGameUI").GetComponent<HudGUIManager>();
            GetComponent<PlayerSceneManager>().LoadScene(playerScene, "FirstSpawn"); //Set start Scene
        }
    }
	
    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetPlayerScene(string scene)
    {
        playerScene = scene;
    }

    void Update()
    {
        if(isLocalPlayer)
        {
            hudGUIManager.UpdateHealthBar(playerCurrentHealth, playerMaxHealth);
        }

    }


    [Command] //This function will run on the server when it is called on the client.
    public void CmdAddPlayerHealth(int amount)
    {
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth + amount, 0, 100);
    }

    public string GetPlayerScene()
    {
        return playerScene;
    }


}
