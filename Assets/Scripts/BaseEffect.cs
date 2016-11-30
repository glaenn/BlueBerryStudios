using UnityEngine;

[CreateAssetMenu(fileName ="StatusEffect", menuName = "StatusEffect/Base")]
public class BaseEffect : ScriptableObject
{
    enum EffectType { Fire, Healing};

    [SerializeField] EffectType effectype;
    [SerializeField][Range(1.0f,600.0f)] private float duration = 2.0f;
    [SerializeField][Range(1,100)] private int power = 5;

    private float secondCounter = 1.0f;

    public string GetEffectType()
    {
        return effectype.ToString();
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
        
    }

    public void UpdateEffect(PlayerData playerData, float time )
    {
        duration -= time;
        secondCounter -= time;

        if(secondCounter <= 0.00f)
        {
            playerData.CmdApplyDamage(power);
            secondCounter = 1.0f;
        }

    }

    public void EndEffect(PlayerData playerData)
    {

    }

}
