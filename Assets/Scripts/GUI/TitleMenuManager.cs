using UnityEngine;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField]    private GameObject[] GUIMenys;
    [SerializeField]    private UnityEngine.UI.Text IPAddress;
    [SerializeField]    private UnityEngine.UI.Text newProfileName;
    [SerializeField]    private UnityEngine.UI.Text[] profileNames;
    [SerializeField]    private UnityEngine.UI.Image[] profileImage;
    private Color newProfileColor = Color.red;
    UnityEngine.Networking.NetworkManager networkManager;

    public void OpenMeny(int menuChoice)
    {
        for (int i = 0; i < GUIMenys.Length; i++)
        {
            GUIMenys[i].SetActive(false);
        }
        GUIMenys[menuChoice].SetActive(true);
    }

    void Start()
    {
        networkManager = UnityEngine.Networking.NetworkManager.singleton;
    }


    private void UpdateProfileGUI()
    {
        if (GameController.instance.GetNumberOfPlayerProfiles() > 0)
        {
            for (int i = 0; i < GameController.instance.GetNumberOfPlayerProfiles(); i++)
            {
                //Get slot
                int slotIndex = GameController.instance.GetSlot(i);
                profileNames[slotIndex].text = GameController.instance.GetPlayerProfileName(i);
                profileImage[slotIndex].color = GameController.instance.GetPlayerProfileColor(i);               
                
            }
        }
    }

    public void StartSinglePlayerGame()
    {
        HostGame();
    }

    public void HostGame()
    {
        UnityEngine.Networking.NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        networkManager.networkAddress = IPAddress.text;
        networkManager.StartClient();
    }

    public void CreateNewProfile(int nextMenu)
    {
        GameController.instance.CreateProfile(newProfileName.text, newProfileColor);
        OpenMeny(nextMenu);        
    }

    public void SetProfileColor(int choice)
    {
        if(choice == 0)
        {
            newProfileColor = Color.red;
        }

        else if(choice == 1)
        {
            newProfileColor = Color.green;
        }

        else if(choice == 2)
        {
            newProfileColor = Color.blue;
        }

        else
        {
            Debug.LogWarning("Invalid profile color!");
        }
    }

    public void DeleteCurrentProfile()
    {
        //Find a way to delete profile card
        
    }

    public void SetMouseSensitivity(float mouseSensitivity)
    {
        GameController.instance.MouseSensitivity = mouseSensitivity;
    }

    public void SetMusicVolyme(float musicVolyme)
    {
        GameController.instance.MusicVolyme = musicVolyme;
    }

    public void SetSoundVolyme(float soundVolyme)
    {
        GameController.instance.SoundVolyme = soundVolyme;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
