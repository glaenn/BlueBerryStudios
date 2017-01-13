using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] string statusEffect;

	// Use this for initialization
	void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(PlayerData.localPlayerInstance.gameObject == other.gameObject)
                other.GetComponent<PlayerData>().CmdSetStatusEffect(statusEffect);
        }   
    }
}
