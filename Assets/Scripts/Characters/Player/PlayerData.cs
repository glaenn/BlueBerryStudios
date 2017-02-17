using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
/// <summary>
/// Holds are the player data
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
    [SyncVar] private float playerMaxStamina = 100;
    [SyncVar] private float playerCurrentStamina = 100;
    private float baseDamage = 5;
    private float meleeRange = 2.0f;
    private bool isSprinting;
    private string playerRespawnScene = "Map01";
    private bool isAlive = true;

    public string GetPlayerScene(){return playerScene;}
    public float GetPlayerStamina() { return playerCurrentStamina; }
    public void SetPlayerSprint(bool isSprinting) { this.isSprinting = isSprinting; }
    public bool IsPlayerSprinting()
    {
        return (isSprinting && playerCurrentStamina > 0);
    }
    public float GetDamage() { return baseDamage; }
    public float GetMeleeRange() { return meleeRange; }

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
            if (playerCurrentStamina < playerMaxStamina && !isSprinting)
                playerCurrentStamina += Time.deltaTime * 10;
            else if (playerCurrentStamina > 0 && isSprinting)
                playerCurrentStamina -= Time.deltaTime * 20;

            playerCurrentStamina = Mathf.Clamp(playerCurrentStamina, 0, playerMaxStamina);

            hudGUIManager.UpdateHealthBar(playerCurrentHealth, playerMaxHealth);
            hudGUIManager.UpdateStaminaBar(playerCurrentStamina, playerMaxStamina);

            if (playerCurrentHealth <= 0 && isAlive)
            {
                isAlive = false;
                GetComponent<PlayerSceneManager>().LoadScene(playerRespawnScene, "Respawn"); //Respawn at latest respawnpoint
                CmdRestoreHealth(playerMaxHealth * 2);
            }
            else if (playerCurrentHealth > 0 && !isAlive)
                isAlive = true;
        }
        if(isServer)
        { 
            for(int i = 0; i < effects.Count; i++)
            {
                effects[i].UpdateEffect(Time.deltaTime);  
                
                if(effects[i].GetEffectDuration() <= 0)
                    RpcRemoveStatusEffect(i);  
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
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth - damage, 0, playerMaxHealth);
        RpcApplyDamage();
    }
    [ClientRpc] //This fuction will run on all clients when called from the server
    public void RpcApplyDamage()
    {
        if (isLocalPlayer)
            hudGUIManager.TakeDamage();
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdRestoreHealth(int health)
    {
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth + health, 0, playerMaxHealth);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetHealth(int health)
    {
        playerCurrentHealth = Mathf.Clamp(health, 0, playerMaxHealth);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdSetStatusEffect(string effectName)
    {
            RpcSetStatusEffect(effectName);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdSendPlayerInteraction(string objectID, int state)
    {
        NetworkSaveData.instance.CmdEditGameData(objectID, state);
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
        effects[effectID].DisableEffect();
        effects.RemoveAt(effectID);
    }
}
