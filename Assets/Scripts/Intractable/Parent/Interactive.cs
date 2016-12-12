using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [SerializeField] protected string objectName;
    public string GetName() { return objectName; }
    public abstract void Activate(GameObject player);
}
