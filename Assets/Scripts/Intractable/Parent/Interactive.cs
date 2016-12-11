using UnityEngine;

public abstract class Interactive : MonoBehaviour
{
    [SerializeField] protected string objectName;
    private bool inUse;

    public string GetName() { return objectName; }
    public abstract void Activate(GameObject player);
}
