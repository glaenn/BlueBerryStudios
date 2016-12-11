using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public sealed class NetworkGameController : NetworkBehaviour
{
    public static NetworkGameController instance = null;
    List<NetworkData> networkData = new List<NetworkData>();

    public delegate void NetworkUpdate();
    public static event NetworkUpdate OnNetworkUpdate;

    [SyncVar]public double serverTime;

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    [Command] //This function will run on the server when it is called on the client.
    public void CmdEditGameData (string objectID, int state)
    {
        RpcUpdateGameData(objectID, state);   
    }

    void Update()
    {
        if (isServer)
            serverTime = Network.time;
    }

   
    [ClientRpc] //This fuction will run on all clients when called from the server
    private void RpcUpdateGameData(string objectID, int state)
    {
        if (!networkData.Exists(x => x.objectID == objectID))
            networkData.Add(new NetworkData(objectID, state, serverTime));
        else
        {
            networkData.Find(x => x.objectID == objectID).gameState = state;
            networkData.Find(x => x.objectID == objectID).serverTimeStamp = serverTime;
        }

        if (OnNetworkUpdate != null)
            OnNetworkUpdate();
    }

    public bool HasGameData(string objectID){ return networkData.Exists(x => x.objectID == objectID);}

    public void GetGameData(string objectID, ref int state, ref double timeStamp)
    {
        NetworkData item = networkData.Find(x => x.objectID == objectID);
        state = item.gameState;
        timeStamp = item.serverTimeStamp;  
    }



  
	
}
