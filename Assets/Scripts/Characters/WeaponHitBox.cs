using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject defaultHitEffect;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("We hit" + collision.gameObject.name);

        if (animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack"))
        {
            Debug.Log("We hit" + collision.gameObject.name);

            Instantiate(defaultHitEffect).transform.position = collision.contacts[0].point;
        }
    }

    void OnTriggerEnter(Collider collider)
    {


    }
	
	// Update is called once per frame
	void Update ()
    {
        if (animator.GetCurrentAnimatorStateInfo(2).IsTag("Attack"))
            GetComponent<BoxCollider>().enabled = true;

        else
            GetComponent<BoxCollider>().enabled = false;
    }
}
