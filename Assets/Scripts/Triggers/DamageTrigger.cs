using UnityEngine;
using System.Collections;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float seconds = 1.0f;

    bool dealDamage = false;

    void Awake()
    {
        StartCoroutine(DamageOverSeconds(seconds));
    }

	// Use this for initialization
	void OnTriggerStay(Collider other)
    {
        new WaitForSeconds(seconds);

	    if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerNetworkData>().CmdAddPlayerHealth(-damage);
        }
	}

    private IEnumerator DamageOverSeconds(float time)
    {
        while(true)
        {
            dealDamage = true;
            yield return 0;
            yield return new WaitForEndOfFrame();
            dealDamage = false;
            yield return new WaitForSeconds(time);
        }

    }


}
