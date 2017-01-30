using System;
using UnityEngine;

public sealed class SetRespawnPoint : Interactive
{
    [SerializeField] string statusEffect;

    // Override the parent Interactive Use funcion
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerData>().CmdSetPlayerRespawnScene(gameObject.scene.name);
        player.GetComponent<PlayerData>().CmdSetStatusEffect(statusEffect);
    }

    protected override void SetToState()
    {
    }
}
