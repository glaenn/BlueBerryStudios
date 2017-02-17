using UnityEngine;

public sealed class PickUpItem : Interactive
{
    public override void Activate(GameObject player)
    {
        state = 1;
        base.Activate(player);
    }

    protected override void SetToState()
    {
        if (!NetworkSaveData.instance.HasGameData(objectID))
            return;

        NetworkSaveData.instance.GetGameData(objectID, ref state, ref serverTimeStamp);

        if (state == 1)
            Destroy(gameObject);
    }  
}
