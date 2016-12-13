using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfileManager : MonoBehaviour
{
    //private string name;


    struct ProfileData
    {
        public string name;
        public float playerMinutes;
        public Color playerColor;

        public string GetName()
        {
            return name;

        }

        public void CreateNewData(string name)
        {

        }
    }

    List<ProfileData> profileData = new List<ProfileData>();

    public void AddProfile()
    {
        profileData.Add(new ProfileData());            
    }

    public void RemoveProfile()
    {
        profileData.RemoveAt(2);
    }

    public void GetProfile(int i)
    {
        ProfileData newProfile = profileData[i];

    }


	
}
