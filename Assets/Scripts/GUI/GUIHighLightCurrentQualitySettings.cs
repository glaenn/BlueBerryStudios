using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHighLightCurrentQualitySettings : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Selectable[] buttons;

    // When the menu is enabled, the correct Graphic setting is selected AND highlighted
    void OnEnable()
    {
        int qualityLevel = QualitySettings.GetQualityLevel();
        UnityEngine.EventSystems.EventSystem eventSystem;
        eventSystem = UnityEngine.EventSystems.EventSystem.current;
        eventSystem.SetSelectedGameObject(buttons[qualityLevel].gameObject, null);
        buttons[qualityLevel].OnSelect(null);
        buttons[qualityLevel].Select();
    }
}
