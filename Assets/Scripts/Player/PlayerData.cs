using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
/// <summary>
/// Holds are the player data.
/// Current Scene
/// Equipment and inventory
/// </summary>

public class PlayerData : NetworkBehaviour
{
    public static PlayerData localPlayerInstance = null;
    private HudGUIManager hudGUIManager;

    [SerializeField] List<BaseEffect> effects = new List<BaseEffect>();

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

        if(isServer)
        {
            for(int i = 0; i < effects.Count; i++)
            {
                effects[i].UpdateEffect(this, Time.deltaTime);  
                
                if(effects[i].GetEffectDuration() <= 0)
                {
                    effects[i].EndEffect(this);
                    effects.RemoveAt(i);
                }
                     
            }

        }
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
    public void CmdSetStatusEffect(BaseEffect effect)
    {
       
            RpcSetStatusEffect(effect);
    }

    [ClientRpc] //This fuction will run on all clients when called from the server
    private void RpcSetStatusEffect(BaseEffect effect)
    {

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].GetEffectType() == effect.GetEffectType())
            {
                if (effects[i].GetEffectDuration() < effect.GetEffectDuration())
                {
                    effects[i].SetEffectDuration(effect.GetEffectDuration());
                }
                return;
            }
        }

        effects.Add(effect);
    }


    public string GetPlayerScene()
    {
        return playerScene;
    }


}
