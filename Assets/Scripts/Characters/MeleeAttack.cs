using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private RaycastHit hit;
    private Interactive interactive;

    // Use this for initialization
    public void PerformMeleeAttack (float damage, float distance)
    {
        if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            if (hit.transform.tag == "Interactable")
            {
                interactive = hit.transform.gameObject.GetComponent<Interactive>();

                try
                {
                    interactive.TakeDamage(transform.parent.gameObject);
                }
                catch
                {
                    Debug.LogError("The object" + hit.transform.name + " has no interactive script placed on it, or is not hittable");
                }
            }
        }

    }
	

}
