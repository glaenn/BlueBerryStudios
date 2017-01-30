using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [SerializeField] private bool updateWithServer;
    [SerializeField] private string objectName; 
    [SerializeField] protected int state = 0;
    [SerializeField] private bool showName;
    protected string objectID;
    protected double serverTimeStamp = 0;

    //If GetName return with an empy string, the hud for interaction will not show
    public string GetName()
    {
        if (showName)
            return objectName;
        else
            return "";
    }

    //If the object is activate by a player (player is the player that activated the object)
    public virtual void Activate(GameObject player)
    {
        if (updateWithServer)
            PlayerData.localPlayerInstance.CmdSendPlayerInteraction(objectID, state);
    }

    public virtual void TakeDamage(GameObject player) //if the object takes damage
    {
        if(updateWithServer)
            PlayerData.localPlayerInstance.CmdSendPlayerInteraction(objectID, state);
    } 

    protected abstract void SetToState();

    void Start()
    {
        if (updateWithServer)
        {
            objectID = gameObject.scene.name + gameObject.name;
            if (NetworkSaveData.instance.HasGameData(objectID))
            {
                SetToState();
            }
            NetworkSaveData.OnNetworkUpdate += new NetworkSaveData.NetworkUpdate(SetToState);
        }
    }

    void OnDestroy()
    {
        if (updateWithServer)
            NetworkSaveData.OnNetworkUpdate -= new NetworkSaveData.NetworkUpdate(SetToState);
    }

   
}
