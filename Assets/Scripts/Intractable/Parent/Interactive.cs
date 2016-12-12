using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [SerializeField] private bool updateWithServer;
    [SerializeField] private string objectName;
    [SerializeField] protected int state = 0;

    protected string objectID;
    
    protected double serverTimeStamp = 0;

    public string GetName() { return objectName; }

    public virtual void Activate(GameObject player)
    {
        if (updateWithServer)
            SendServerCommands();
    }
    protected abstract void SendServerCommands();

    void Start()
    {
        if (updateWithServer)
        {
            objectID = gameObject.scene.name + gameObject.name;
            if (NetworkGameController.instance.HasGameData(objectID))
            {
                GetState();
            }
            NetworkGameController.OnNetworkUpdate += new NetworkGameController.NetworkUpdate(GetState);
        }

    }

    void OnDestroy()
    {
        if (updateWithServer)
            NetworkGameController.OnNetworkUpdate -= new NetworkGameController.NetworkUpdate(GetState);
    }

    abstract protected void GetState();

}
