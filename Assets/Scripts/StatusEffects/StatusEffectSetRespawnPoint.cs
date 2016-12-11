using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "StatusEffect/SetRespawnPoint")]
public class StatusEffectSetRespawnPoint : BaseStatusEffect
{
    public override void StartEffect(PlayerData playerData)
    {
        playerData.CmdSetHealth(1000);
        base.StartEffect(playerData);
    }

    public override void UpdateEffect(float time)
    {
        duration -= time;
    }
}
