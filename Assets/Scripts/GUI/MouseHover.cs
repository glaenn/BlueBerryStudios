using UnityEngine;
using UnityEngine.SceneManagement;


public class MouseHover : MonoBehaviour
{
    public Animator menuAnim;

    [SerializeField] private GameObject[] hiddenGameObject;
    [SerializeField] private GameObject[] selectGameObject; 
    //[SerializeField] private Scene sceneSelection;     

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
        if (!isClicked)
            return;

        Debug.Log("Clicked " + selectGameObject);
        foreach (GameObject selectObject in selectGameObject)
        {
            selectObject.SetActive(isClicked);
            Debug.Log(gameObject + "was clicked");                   
                       
        }

        //SceneManager.LoadScene("NetworkScene");
        menuAnim.SetTrigger("gotoOptions");
        Debug.Log("Not clicked anymore " + selectGameObject);       

    }

    

    

    

    

}
