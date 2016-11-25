using UnityEngine;
using System.Collections;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] int damage = 1;

	// Use this for initialization
	void OnTriggerStay(Collider other)
    {
	    if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerNetworkData>().CmdAddPlayerHealth(-damage);
        }
	}
	
}
