using UnityEngine;

public class DestroyableObject : Interactive
{
    public override void TakeDamage(GameObject player, int damage)
    {
        state += damage;
        base.TakeDamage(player, damage);
    }

    protected override void SetToState()
    {
        if (!NetworkSaveData.instance.HasGameData(objectID))
            return;

        NetworkSaveData.instance.GetGameData(objectID, ref state, ref serverTimeStamp);

        if (state > 0)
            Destroy(gameObject);
    }
}
