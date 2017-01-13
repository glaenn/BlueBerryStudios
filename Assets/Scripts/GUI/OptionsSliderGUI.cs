using UnityEngine;

public class OptionsSliderGUI : MonoBehaviour
{
    enum Option { fov, brightness, gameVol, musicVol, mouseSens};

    [SerializeField]
    Option options;

   void Awake()
   {
        switch (options)
        {
            case Option.fov:
                GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.FieldOfView;
                break;

            case Option.gameVol:
                GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.SoundVolume;
                break;

            case Option.musicVol:
                GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.MusicVolume;
                break;

            case Option.mouseSens:
                GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.MouseSensitivity;
                break;
        }     
   }

    public void OnSliderChange()
    {
        switch (options)
        {
            case Option.fov:
                GameController.instance.FieldOfView = GetComponent<UnityEngine.UI.Slider>().value;
                break;

            case Option.gameVol:
                GameController.instance.SoundVolume = GetComponent<UnityEngine.UI.Slider>().value;
                break;

            case Option.musicVol:
                GameController.instance.MusicVolume = GetComponent<UnityEngine.UI.Slider>().value;
                break;

            case Option.mouseSens:
                GameController.instance.MouseSensitivity = GetComponent<UnityEngine.UI.Slider>().value;
                break;
        }
    }

}
