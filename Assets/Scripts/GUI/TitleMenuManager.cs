using UnityEngine;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField]    private GameObject[] GUIMenys;
    [SerializeField]    private UnityEngine.UI.Text IPAddress;
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

    public void CreateNewProfile()
    {
        //Find a way to create a profile
        
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
