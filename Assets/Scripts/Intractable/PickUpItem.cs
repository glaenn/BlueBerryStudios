﻿using System;
using UnityEngine;

public sealed class PickUpItem : Interactive
{
    protected override void SendServerCommands()
    {
        PlayerData.localPlayerInstance.CmdSendPlayerInteraction(objectID, 1);  
    }

    protected override void GetState()
    {
        if(NetworkGameController.instance.HasGameData(objectID))
            NetworkGameController.instance.GetGameData(objectID, ref state, ref serverTimeStamp);

        if (state == 1)
            Destroy(gameObject);
    }

    
}
