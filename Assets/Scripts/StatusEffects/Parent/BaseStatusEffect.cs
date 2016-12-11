using UnityEngine;

public abstract class BaseStatusEffect : ScriptableObject
{
    enum StatusEffectType { None, FireDmg, Healing};
    [SerializeField] private StatusEffectType statusEffectType;
    [SerializeField][Range(1.0f,600.0f)] protected float duration = 2.0f;
    [SerializeField][Range(1,100)] protected int power = 5;
    [SerializeField] private string visualEffectName;

    private GameObject effectPlayerInstance;
    protected PlayerData playerData;
    protected float secondCounter = 1.0f;

    public string GetEffectType(){ return statusEffectType.ToString();}
    public float GetEffectPower(){return power; }
    public float GetEffectDuration(){return duration;}
    public void SetEffectDuration(float time){duration = time;}

    public abstract void UpdateEffect(float time);


    public virtual void StartEffect(PlayerData playerData)
    {
        this.playerData = playerData;
        effectPlayerInstance = Instantiate(Resources.Load<GameObject>("EffectPlayer"), playerData.transform, false);
        effectPlayerInstance.GetComponent<EffectPlayer>().InitiateEffectPlayer(visualEffectName);
    }

    public void DisableEffect()
    {
        effectPlayerInstance.GetComponent<EffectPlayer>().DisableEffect();
    }

}
