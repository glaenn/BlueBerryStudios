
using UnityEngine;

public class DestroyableObject : Interactive
{
    public override void TakeDamage(GameObject player)
    {
        state = 1;
        base.TakeDamage(player);
    }

    protected override void SetToState()
    {
        if (state == 1)
            Destroy(gameObject);
    }
}
