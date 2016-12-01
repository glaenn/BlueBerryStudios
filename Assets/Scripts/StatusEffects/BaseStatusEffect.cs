using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName ="StatusEffect", menuName = "StatusEffect/Base")]
public class BaseStatusEffect : ScriptableObject
{
    enum StatusEffectType { Fire, Healing};

    [SerializeField] StatusEffectType statusEffectType;
    [SerializeField][Range(1.0f,600.0f)] private float duration = 2.0f;
    [SerializeField][Range(1,100)] private int power = 5;

    private float secondCounter = 1.0f;
    [SerializeField] string effectPlayer;

    GameObject effectPlayerInstance;

    PlayerData playerData;

    public string GetEffectType()
    {
        return statusEffectType.ToString();
    }

    public float GetEffectPower()
    {
        return power;
    }

    public float GetEffectDuration()
    {
        return duration;
    }

    public void SetEffectDuration(float time)
    {
        duration = time;
    }

    public void StartEffect(PlayerData playerData)
    {
        this.playerData = playerData;
        effectPlayerInstance = Instantiate(Resources.Load<GameObject>("EffectPlayers/" + effectPlayer), playerData.transform, false);
    }
    
    public void UpdateEffect(float time )
    {
        duration -= time;
        secondCounter -= time;

        if(secondCounter <= 0.00f)
        {
            playerData.CmdApplyDamage(power);
            secondCounter = 1.0f;
        }
    }

    public void EndEffect()
    {
        Destroy(effectPlayerInstance);
    }

}
