using UnityEngine;
using System.Collections.Generic;

public sealed class GameController : MonoBehaviour
{
    //Localdata
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

    //Profiles
    private List<ProfileSaveData> profiles = new List<ProfileSaveData>();

    public int GetNumberOfPlayerProfiles() { return profiles.Count; }
    public string GetPlayerProfileName(int profileID) { return profiles[profileID].profileName; }
    public void SetPlayerProfileName(int profileID, string profileName) {profiles[profileID].profileName = profileName; }
    public Color GetPlayerProfileColor(int profileID) { return profiles[profileID].profileColor; }
    public void SetPlayerProfileColor(int profileID, Color profileColor) { profiles[profileID].profileColor = profileColor; }

    public void CreateProfile(string profileName, Color profileColor)
    {
        profiles.Add(new ProfileSaveData(profileName, profileColor));
    }
    public void RemoveProfile(int profileID)
    {
        profiles.RemoveAt(profileID);
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