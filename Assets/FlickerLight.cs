using UnityEngine;
using System.Collections;

public class FlickerLight : MonoBehaviour
{
    //Do not use 

    [SerializeField] private float minFlickerIntensity = 0.5f;
    [SerializeField] private float maxFlickerIntensity = 1f;
    [SerializeField] private float flickerSpeed = 0.035f;
    [SerializeField] private float timer = 2f;


    [SerializeField]
    ParticleSystem.ShapeModule particleShape;    

    void Start()
    {
        particleShape = GetComponent<ParticleSystem.ShapeModule>();
    }

    void Update()
    {
        if (timer > flickerSpeed)
        {
            particleShape.radius = Random.Range(minFlickerIntensity, maxFlickerIntensity);
            timer = 0;
        }

        else
        {
            timer += Time.deltaTime;
        }
    }

    



}
