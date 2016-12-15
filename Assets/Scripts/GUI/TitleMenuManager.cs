using UnityEngine;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField]    private GameObject[] GUIMenys;
    [SerializeField]    private UnityEngine.UI.Text IPAddress;
    [SerializeField]    private UnityEngine.UI.Text newProfileName;

    [System.Serializable]
    struct ProfileSlotGraphics
    {
        public  UnityEngine.UI.Text profileName;
        public UnityEngine.UI.Image profileImage;        
    }

    [SerializeField]
    ProfileSlotGraphics[] profileSlotGraphics;

    [SerializeField] Sprite emptySlot;
    [SerializeField] Sprite filledSlot;
 
    private Color newProfileColor = Color.red;
    
    private int lastSelectedProfile = 0;    

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
        for(int i = 0; i < profileSlotGraphics.Length; i++)
        {
            profileSlotGraphics[i].profileName.text = "";
            profileSlotGraphics[i].profileImage.color = Color.white;
            profileSlotGraphics[i].profileImage.sprite = emptySlot;
        }


        if (GameController.instance.GetNumberOfProfiles() > 0)
        {
            for (int i = 0; i < GameController.instance.GetNumberOfProfiles(); i++)
            {
                //Get slot
                int slotIndex = GameController.instance.GetProfileSlot(i);
                profileSlotGraphics[slotIndex].profileName.text = GameController.instance.GetProfileName(i);
                profileSlotGraphics[slotIndex].profileImage.color = GameController.instance.GetProfileColor(i);
                profileSlotGraphics[slotIndex].profileImage.sprite = filledSlot;

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

    public void OpenCreateNewProfile(int profile)
    {
        lastSelectedProfile = profile;
        OpenMeny(2);
    }

    public void CreateNewProfile(int nextMenu)
    {
        GameController.instance.CreateProfile(newProfileName.text, newProfileColor, lastSelectedProfile);
        OpenMeny(nextMenu);
        UpdateProfileGUI();        
    }

    public void SetProfileColor(int choice)
    {
        if(choice == 0)
        {
            
            newProfileColor = Color.red;
            UpdateProfileGUI();
        }

        else if(choice == 1)
        {
            newProfileColor = Color.green;
            UpdateProfileGUI();
        }

        else if(choice == 2)
        {
            newProfileColor = Color.blue;
            UpdateProfileGUI();
        }

        else
        {
            Debug.LogWarning("Invalid profile color!");
        }
    }
  

    public void DeleteCurrentProfile(int profileID)
    {
        GameController.instance.RemoveProfile(profileID);
        UpdateProfileGUI();
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
