using UnityEngine;

public class SetRespawnPoint : Interactive
{
    // Override the parent Interactive Use funcion
    public override void Use(GameObject player = null)
    {
        player.GetComponent<PlayerData>().CmdSetPlayerRespawnScene(gameObject.scene.name);
    }
}
