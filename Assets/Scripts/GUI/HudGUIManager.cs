using UnityEngine;

public class HudGUIManager : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Text nameText;
    [SerializeField]
    private GameObject textBG;

    // Use this for initialization
    public void ShowInteractionText (string nameOfObject)
    { 
        nameText.text = nameOfObject;
        textBG.SetActive(true); 
	}

    public void HideInteractionText()
    {
        textBG.SetActive(false);
    }


}
