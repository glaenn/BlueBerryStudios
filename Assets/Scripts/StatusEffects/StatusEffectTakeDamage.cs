using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "StatusEffect/TakeDamage")]
public class StatusEffectTakeDamage : BaseStatusEffect
{
    public override void UpdateEffect(float time)
    {
        if (duration <= 0)
            return;

        duration -= time;
        secondCounter -= time;

        if (secondCounter <= 0.00f)
        {
            playerData.CmdApplyDamage(power);
            secondCounter = 1.0f;
        }
    }

}
