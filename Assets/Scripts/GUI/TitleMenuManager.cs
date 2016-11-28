using UnityEngine;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField]    private GameObject[] GUIMenys;
    [SerializeField]    private UnityEngine.UI.Text IPAddress;
    UnityEngine.Networking.NetworkManager networkManager;
    private GameController gameController;

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
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void StartGame()
    {
        UnityEngine.Networking.NetworkManager.singleton.StartHost();
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
        gameController.MouseSensitivity = mouseSensitivity;
    }

    public void SetMusicVolyme(float musicVolyme)
    {
        gameController.MusicVolyme = musicVolyme;
    }

    public void SetSoundVolyme(float soundVolyme)
    {
        gameController.SoundVolyme = soundVolyme;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
