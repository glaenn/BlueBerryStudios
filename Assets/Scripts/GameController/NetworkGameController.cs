using UnityEngine;
using UnityEngine.Networking;

public sealed class NetworkGameController : NetworkBehaviour
{
    public struct NetData
    {
        public string objID;
        public int gameState;
        public double serverTimeStamp;

        public NetData(string objectID, int gameState, double serverTimeStamp)
        {
            objID = objectID;
            this.gameState = gameState;
            this.serverTimeStamp = serverTimeStamp;
        }
    }

    public static NetworkGameController instance = null;
    public delegate void NetworkUpdate();
    public static event NetworkUpdate OnNetworkUpdate;
    public class NetworkData : SyncListStruct<NetData>{}
    NetworkData networkData = new NetworkData();
    [SyncVar]public double serverTime;

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Update()
    {
        if (isServer)
            serverTime = Network.time;
    }

    [Command]
    public void CmdEditGameData(string objectID, int state)
    {
        if (!HasGameData(objectID))
            networkData.Add(new NetData(objectID, state, serverTime));
        else
        {
            networkData[GetDataIndex(objectID)] = new NetData(objectID, state, serverTime);
        }
        RpcUpdateGameData(objectID, state);
    }

    [ClientRpc] //This fuction will run on all clients when called from the server
    private void RpcUpdateGameData(string objectID, int state)
    {
        if (OnNetworkUpdate != null)
            OnNetworkUpdate();
    }
    public bool HasGameData(string objectID)
    {
        for (int i = 0; i < networkData.Count; i++)
        {
            if (networkData[i].objID == objectID)
                return true;
        }
        return false;
    }
    private int GetDataIndex(string objectID)
    {
        for (int i = 0; i < networkData.Count; i++)
        {
            if (networkData[i].objID == objectID)
                return i;
        }

        return 0;
    }
    public void GetGameData(string objectID, ref int state, ref double timeStamp)
    {
        state = networkData[GetDataIndex(objectID)].gameState;
        timeStamp = networkData[GetDataIndex(objectID)].serverTimeStamp;
    }
}
