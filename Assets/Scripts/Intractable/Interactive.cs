using UnityEngine;

public class Interactive : MonoBehaviour
{
    [SerializeField] protected string objectName;

	public virtual void Use ()
    {
        Debug.Log("Use");
	}

    public virtual string GetName()
    {
        return objectName;
    }
}
