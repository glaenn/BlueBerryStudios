using UnityEngine;

public class HudGUIManager : MonoBehaviour
{
    [SerializeField]private UnityEngine.UI.Text nameText;
    [SerializeField]private GameObject textBG;
    [SerializeField]private RectTransform healthbar;

    private float healthBarMaxSize;

    void Start()
    {
        healthBarMaxSize = healthbar.sizeDelta.x;
    }

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

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthbar.sizeDelta = new Vector2((currentHealth / maxHealth) * healthBarMaxSize, healthbar.sizeDelta.y);

    }
}
