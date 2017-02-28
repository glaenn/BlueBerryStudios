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

    public void Test(GameObject t)
    {
        Activate(t);
    }

    //If the object is activate by a player (player is the player that activated the object)
    public virtual void Activate(GameObject player)
    {
        if (updateWithServer)
            PlayerData.localPlayerInstance.CmdSendPlayerInteraction(objectID, state);
    }
    //if the object recieves damage
    public virtual void TakeDamage(GameObject player, int damage) 
    {
        if(updateWithServer)
            PlayerData.localPlayerInstance.CmdSendPlayerInteraction(objectID, state);
    } 

    protected virtual void SetToState()
    {
        if(updateWithServer)
        {
            if (!NetworkSaveData.instance.HasGameData(objectID))
                return;

            NetworkSaveData.instance.GetGameData(objectID, ref state, ref serverTimeStamp);
        }
    }

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
