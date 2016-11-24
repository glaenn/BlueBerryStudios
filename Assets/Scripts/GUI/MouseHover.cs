using UnityEngine;


public class MouseHover : MonoBehaviour
{
    [SerializeField] private GameObject[] hiddenGameObject;
    [SerializeField] private GameObject[] selectGameObject;    

    void Start()
    {
        SetVisibile(false);
        ConfirmSelection(false);        
    }

    public void SetVisibile(bool isVisible)
    {
        Debug.Log("DERP!");
        foreach (GameObject hiddenObject in hiddenGameObject)
        {
            hiddenObject.SetActive(isVisible);
        }
    }

    public void ConfirmSelection(bool isClicked)
    {
        Debug.Log("Clicked " + selectGameObject);
        foreach (GameObject selectObject in selectGameObject)
        {
            selectObject.SetActive(isClicked);
        }

    }

    

    

}
