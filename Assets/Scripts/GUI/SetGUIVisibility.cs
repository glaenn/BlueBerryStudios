using UnityEngine;
 
public class SetGUIVisibility : MonoBehaviour
{
    [SerializeField] private GameObject[] hiddenGameObject;

    void Start()
    {
        SetVisibile(false);   
    }

    public void SetVisibile(bool isVisible)
    {
        foreach (GameObject hiddenObject in hiddenGameObject)
        {
            hiddenObject.SetActive(isVisible);
        }
    }
}
