using UnityEngine;
using System.Collections;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] string statusEffect;

	// Use this for initialization
	void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerData>().CmdSetStatusEffect(statusEffect);
        }

    }
}
