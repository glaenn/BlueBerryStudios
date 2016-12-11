using UnityEngine;

public abstract class InteractiveServer : Interactive
{
    protected string objectID;
    [SerializeField]protected int state = 0;
    protected double serverTimeStamp = 0;

    // Update is called once per frame
    void Start()
    {
        objectID = gameObject.scene.name + objectName + gameObject.name;
        if( NetworkGameController.instance.HasGameData(objectID))
        {  
            GetState();
        }
        NetworkGameController.OnNetworkUpdate += new NetworkGameController.NetworkUpdate(GetState); 
    }

    protected void SendNetworkData(int state)
    {
        NetworkGameController.instance.CmdEditGameData(objectID,  state);
    }

    void OnDestroy()
    {
        NetworkGameController.OnNetworkUpdate -= new NetworkGameController.NetworkUpdate(GetState);
    }


    abstract protected void GetState();

}
