using UnityEngine;

public class Interactive : MonoBehaviour
{
    [SerializeField] protected string objectName;

	public virtual void Use (GameObject player = null)
    {
        //Do nothing
	}

    public string GetName()
    {
        return objectName;
    }
}
