using UnityEngine;

public sealed class GameMenuManager : MonoBehaviour
{
    [SerializeField]    private GameObject[] GUIMenus;
    [SerializeField]    private UnityEngine.UI.Text IPAddress;
    [SerializeField]    private UnityEngine.UI.Text newProfileName;
    [SerializeField]    ProfileSlotGraphics[] profileSlotGraphics;
    [SerializeField]    Sprite emptySlot;
    [SerializeField]    Sprite filledSlot;

    [System.Serializable]
    struct ProfileSlotGraphics
    {
        public UnityEngine.UI.Text profileName;
        public UnityEngine.UI.Image profileImage;        
    }      

    private Color newProfileColor = Color.red;
    private int lastSelectedProfile = 0;    
    private UnityEngine.Networking.NetworkManager networkManager;

    public void ToggleMenu(int menuChoice)
    {
        ToggleMenu(menuChoice, true);
    }

    public void ToggleMenu(int menuChoice, bool menuOpen = true)
    {
        for (int i = 0; i < GUIMenus.Length; i++)
        {
            GUIMenus[i].SetActive(false);
        }

        if(menuOpen)
        GUIMenus[menuChoice].SetActive(true);
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
        ToggleMenu(2);
    }

    public void CreateNewProfile(int nextMenu)
    {
        GameController.instance.CreateProfile(newProfileName.text, newProfileColor, lastSelectedProfile);
        ToggleMenu(nextMenu);
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
  

    public void SetInvertedVerticalAxis(bool invertedVerticalAxis)
    {
        GameController.instance.InvertedVerticalAxis = invertedVerticalAxis;
    }

    public void SetGraphicsLevel(int qualityLevel)
    {
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    public void GoBackToMainMenu()
    {
        

        if (UnityEngine.Networking.NetworkServer.active)
            UnityEngine.Networking.NetworkManager.singleton.StopHost();
        else
            UnityEngine.Networking.NetworkManager.singleton.StopClient();
    }
    

    public void QuitGame()
    {
        Application.Quit();
    }

}
