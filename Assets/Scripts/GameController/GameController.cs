using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private float mouseSensitivity = 0.5f;
    private float musicVolyme = 1.0f;                      
    private float soundVolyme = 1.0f;                      

    public float MouseSensitivity                     
    {
       get 
       { 
          return mouseSensitivity; 
       }
       set 
       {
            mouseSensitivity = value; 
       }
    }
    public float MusicVolyme
    {
        get
        {
            return musicVolyme;
        }
        set
        {
            musicVolyme = value;
        }
    }
    public float SoundVolyme
    {
        get
        {
            return soundVolyme;
        }
        set
        {
            soundVolyme = value;
        }
    }

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


}