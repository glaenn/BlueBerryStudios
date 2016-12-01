using UnityEngine;

[CreateAssetMenu(fileName ="StatusEffect", menuName = "StatusEffect/Base")]
public class BaseStatusEffect : ScriptableObject
{
    enum StatusEffectType { Fire, Healing};

    [SerializeField] StatusEffectType statusEffectType;
    [SerializeField][Range(1.0f,600.0f)] private float duration = 2.0f;
    [SerializeField][Range(1,100)] private int power = 5;

    private float secondCounter;
    [SerializeField] GameObject visualEffect;
    GameObject visualEffectInstance;
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
        //visualEffectInstance = Instantiate(visualEffect);
        //visualEffectInstance.transform.parent = playerData.transform.parent;
         secondCounter = 1.0f;
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
