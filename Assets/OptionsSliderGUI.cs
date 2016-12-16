using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsSliderGUI : MonoBehaviour
{
    enum Option { fov, brightness, gameVol, musicVol, mouseSens};

    [SerializeField]
    Option options;


    

   void Awake()
   {
        if(options == Option.fov)
        {
            GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.FieldOfView;
        }

        if(options == Option.brightness)
        {
            GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.Brightness;
        }

        if(options == Option.gameVol)
        {
            GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.SoundVolyme;
        }

        if(options == Option.musicVol)
        {
            GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.MusicVolyme;       
        }

        if(options == Option.mouseSens)
        {
            GetComponent<UnityEngine.UI.Slider>().value = GameController.instance.MouseSensitivity;
        }            
   } 

    public void OnSliderChange()
    {
        if (options == Option.fov)
        {
            GameController.instance.FieldOfView = GetComponent<UnityEngine.UI.Slider>().value;
        }

        if (options == Option.brightness)
        {
            GameController.instance.Brightness = GetComponent<UnityEngine.UI.Slider>().value;
        }

        if (options == Option.gameVol)
        {
            GameController.instance.SoundVolyme = GetComponent<UnityEngine.UI.Slider>().value;
        }

        if (options == Option.musicVol)
        {
            GameController.instance.MusicVolyme = GetComponent<UnityEngine.UI.Slider>().value;
       
                              
        }

        if (options == Option.mouseSens)
        {
            GameController.instance.MouseSensitivity = GetComponent<UnityEngine.UI.Slider>().value;
        }

    }


    
     

    


}
