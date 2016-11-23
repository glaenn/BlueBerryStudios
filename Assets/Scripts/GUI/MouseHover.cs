using UnityEngine;


public class MouseHover : MonoBehaviour
{
    [SerializeField] private GameObject[] hiddenGameObject;    

    void Start()
    {
        SetVisibile(false);
    }

    public void SetVisibile(bool isVisible)
    {
        Debug.Log("DERP!");
        foreach (GameObject hiddenObject in hiddenGameObject)
        {
            hiddenObject.SetActive(isVisible);
        }

    }

   


}
