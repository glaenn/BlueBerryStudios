using UnityEngine;

public class GUISetDefaultSelected : MonoBehaviour
{
    // When the menu is activated, the selectable ui element that is attached to this component is selected AND highlighted
    void OnEnable()
    {
        UnityEngine.EventSystems.EventSystem eventSystem;
        eventSystem = UnityEngine.EventSystems.EventSystem.current;
        eventSystem.SetSelectedGameObject(gameObject, null);
        UnityEngine.UI.Selectable selactable = gameObject.GetComponent<UnityEngine.UI.Selectable>();        
        selactable.OnSelect(null);
        selactable.Select();
    }
}
