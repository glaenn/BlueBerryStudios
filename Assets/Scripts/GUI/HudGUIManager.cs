using UnityEngine;
using System.Collections;

public class HudGUIManager : MonoBehaviour
{
    [SerializeField]private UnityEngine.UI.Text nameText;
    [SerializeField]private GameObject textBG;
    [SerializeField]private RectTransform healthbar;
    [SerializeField]private RectTransform staminabar;
    [SerializeField]private UnityEngine.UI.RawImage blackFade;
    [SerializeField]private UnityEngine.UI.RawImage damageEffect;

    private bool hasTakenDamage = true;

    private float healthBarMaxSize;
    private float staminaBarMaxSize;

    void Start()
    {
        healthBarMaxSize = healthbar.sizeDelta.x;
        staminaBarMaxSize = staminabar.sizeDelta.x;
    }

    // Use this for initialization
    public void ShowInteractionText (bool show, string nameOfObject = "")
    {
        if (nameOfObject == "")
        {
            textBG.SetActive(false);
            return;
        }

        textBG.SetActive(show);
        nameText.text = nameOfObject;  
	}

    public void SetScreenFade(bool fade)
    {
        if (!fade)
            StartCoroutine(StartFade());
        else
            blackFade.color = new Color(0, 0, 0, 1);
    }

    public void TakeDamage()
    {
        if(hasTakenDamage)
            StartCoroutine(StartDamageEffect());
    }

    private IEnumerator StartDamageEffect()
    {
        hasTakenDamage = false;

        while (damageEffect.color.a < 0.5)
        {
            yield return new WaitForSeconds(0.005f);
            damageEffect.color += new Color(0, 0, 0, 0.1f);
        }
        while (damageEffect.color.a > 0)
        {
            yield return new WaitForSeconds(0.01f);
            damageEffect.color -= new Color(0, 0, 0, 0.05f);
        }

        hasTakenDamage = true;
    }

    private IEnumerator StartFade()
    {
        while (blackFade.color.a > 0.0f)
        {
            blackFade.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
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
