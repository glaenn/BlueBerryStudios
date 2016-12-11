using System.Collections;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    GameObject visualEffectInstance;
    private bool effectHasEnded = false;

    public void InitiateEffectPlayer(string visualEffectName)
    {
        StartCoroutine(LoadEffects(visualEffectName));
    }

    IEnumerator LoadEffects(string visualEffectName)
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>("VisualStatusEffects/" + visualEffectName);
        yield return request;
        visualEffectInstance = Instantiate(request.asset, transform) as GameObject;    
    }

    public bool EffectHasEnded()
    {
        return effectHasEnded;
    }

    public void DisableEffect()
    {
        StartCoroutine(DisableEffectCoroutine());
    }

    IEnumerator DisableEffectCoroutine()
    {
        //Has the effect even started yet?
        while (visualEffectInstance == null)
        {
            yield return null;
        }

        ParticleSystem particles = visualEffectInstance.GetComponent<ParticleSystem>();
        particles.Stop();

        while(particles.particleCount > 0)
        {
            yield return null;
        }

        Destroy(gameObject);

    }

    void OnDestroy()
    {
        Destroy(visualEffectInstance);
    }
}
