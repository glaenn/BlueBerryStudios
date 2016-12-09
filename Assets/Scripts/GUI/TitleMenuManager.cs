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

    public void StartGame()
    {
        UnityEngine.Networking.NetworkManager.singleton.StartHost();
        //Start up Loading Screen
        Debug.Log("Clicked");
        //Game fades in and out
        //Scene is retreived
        //Scene is started
    }

    public void HostGame()
    {
        UnityEngine.Networking.NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {

        //IPAddress.text = "localhost";

        networkManager.networkAddress = IPAddress.text;
        networkManager.StartClient();
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
