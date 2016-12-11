using UnityEngine;

public sealed class PickUpItem : InteractiveServer
{
    public override void Activate(GameObject player)
    {
        SendNetworkData(1);
    }

    protected override void GetState()
    {
        if(NetworkGameController.instance.HasGameData(objectID))
            NetworkGameController.instance.GetGameData(objectID, ref state, ref serverTimeStamp);

        if (state == 1)
            Destroy(gameObject);
    }

    
}
