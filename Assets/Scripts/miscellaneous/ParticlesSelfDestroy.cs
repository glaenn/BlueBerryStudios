using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlesSelfDestroy : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
        if (GetComponent<ParticleSystem>().isStopped)
            Destroy(gameObject);
	}
}
