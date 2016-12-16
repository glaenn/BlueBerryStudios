using UnityEngine;
using System.Collections;

public class HudGUIManager : MonoBehaviour
{
    [SerializeField]private UnityEngine.UI.Text nameText;
    [SerializeField]private GameObject textBG;
    [SerializeField]private RectTransform healthbar;
    [SerializeField]private RectTransform staminabar;
    [SerializeField]private UnityEngine.UI.RawImage blackFade;

    private bool hasFaded = true;

    private float healthBarMaxSize;
    private float staminaBarMaxSize;

    void Start()
    {
        healthBarMaxSize = healthbar.sizeDelta.x;
        staminaBarMaxSize = staminabar.sizeDelta.x;
    }

    // Use this for initialization
    public void ShowInteractionText (string nameOfObject)
    { 
        nameText.text = nameOfObject;
        textBG.SetActive(true); 
	}

    public void SetScreenFade(bool fade)
    {
        if (fade)
            StartCoroutine(StartFade());
        else
            hasFaded = true;
    }

    private IEnumerator StartFade()
    {
        blackFade.color = new Color(0, 0, 0, 1.0f);

        while(!hasFaded)
        {
            yield return new WaitForFixedUpdate();
        }
        while (blackFade.color.a > 0.0f)
        {
            blackFade.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForFixedUpdate();
            if (blackFade.color.a < 0.02)
                blackFade.color = new Color(0, 0, 0, 0);
        }
        hasFaded = false;

    }

    public void HideInteractionText()
    {
        textBG.SetActive(false);
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthbar.sizeDelta = new Vector2((currentHealth / maxHealth) * healthBarMaxSize, healthbar.sizeDelta.y);
    }

    public void UpdateStaminaBar(float currentStamina, float maxStamina)
    {
        staminabar.sizeDelta = new Vector2((currentStamina / maxStamina) * staminaBarMaxSize, staminabar.sizeDelta.y);
    }
}
