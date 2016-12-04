using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
/// <summary>
/// Holds are the player data.
/// Current Scene
/// Equipment and inventory
/// </summary>

[RequireComponent(typeof(PlayerSceneManager))]
public class PlayerData : NetworkBehaviour
{
    public static PlayerData localPlayerInstance = null;
    private HudGUIManager hudGUIManager;

    [SerializeField] List<BaseStatusEffect> effects = new List<BaseStatusEffect>();

    //Player variables
    [SyncVar] private string playerScene = "Map01"; //SyncVar makes sure that the server updates the variable to the clients
    [SyncVar] private int playerMaxHealth = 100;
    [SyncVar] private int playerCurrentHealth = 100;
    [SyncVar] private string playerRespawnScene = "Map01";

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

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            hudGUIManager.UpdateHealthBar(playerCurrentHealth, playerMaxHealth);

            if(playerCurrentHealth <= 0)
            {
                GetComponent<PlayerSceneManager>().LoadScene(playerRespawnScene, "Respawn"); //Respawn at latest respawnpoint
                CmdRestoreHealth(playerMaxHealth * 2);
            }
        }
        if(isServer)
        { 
            for(int i = 0; i < effects.Count; i++)
            {
                effects[i].UpdateEffect(Time.deltaTime);  
                
                if(effects[i].GetEffectDuration() <= 0)
                {
                    RpcRemoveStatusEffect(i);
                }      
            }
        }
    }
    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetPlayerRespawnScene(string mapName)
    {
        playerRespawnScene = mapName;
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdApplyDamage(int damage)
    {
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth - damage, 0, 100);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdRestoreHealth(int health)
    {
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth + health, 0, 100);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetStatusEffect(string effectName)
    {
            RpcSetStatusEffect(effectName);
    }

    [ClientRpc] //This fuction will run on all clients when called from the server
    private void RpcSetStatusEffect(string effectName)
    {
        BaseStatusEffect newEffect = Instantiate(Resources.Load<BaseStatusEffect>("StatusEffects/" + effectName));

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].GetEffectType() == newEffect.GetEffectType())
            {
                if (effects[i].GetEffectDuration() < newEffect.GetEffectDuration())
                {
                    effects[i].SetEffectDuration(newEffect.GetEffectDuration());
                }
                return;
            }
        }
        effects.Add(newEffect);
        newEffect.StartEffect(this);
    }
    [ClientRpc] //This fuction will run on all clients when called from the server
    private void RpcRemoveStatusEffect(int effectID)
    {
        effects[effectID].EndEffect();
        effects.RemoveAt(effectID);
    }

    public string GetPlayerScene()
    {
        return playerScene;
    }

}
