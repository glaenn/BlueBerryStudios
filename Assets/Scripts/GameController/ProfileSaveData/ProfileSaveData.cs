using UnityEngine;

public class ProfileSaveData
{
    public string profileName;
    public Color profileColor;
    private int uniquID; //For server identify purposes
    
    public ProfileSaveData(string profileName, Color profileColor)
    {
        this.profileName = profileName;
        this.profileColor = profileColor;
        Random.InitState((int)(Time.realtimeSinceStartup * 10) + int.Parse(profileName)); 
        uniquID = Random.Range(0, 1000000);
    }
	
}
