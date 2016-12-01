using System.Collections;
using UnityEngine;

public class EffectLoader : MonoBehaviour
{
    [SerializeField] string visualEffectName;
    GameObject visualEffectInstance;

    // Use this for initialization
    void Start ()
    {
        StartCoroutine(LoadEffects());
    }

    IEnumerator LoadEffects()
    {
        ResourceRequest request = Resources.LoadAsync<GameObject>("Effects/" + visualEffectName);
        yield return request;
        visualEffectInstance = Instantiate(request.asset, transform) as GameObject;
        
    }

    void OnDestroy()
    {
        Destroy(visualEffectInstance);
    }
}
