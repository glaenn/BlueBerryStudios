using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public static MusicController instance = null;   //Static instance of GameManager which allows it to be accessed by any other script.
    private AudioSource audioSource;

    // Use this for initialization //Is on start to be sure that it loads after gamecontroller
    void  Start ()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        GameController.OnMusicVolumeUpdate += new GameController.MusicVolumeUpdate(ChangeVolume);
    }
	
	// Update is called once per frame
	void ChangeVolume (float volume)
    {
        audioSource.volume = volume;
	}

    void OnDestroy()
    {
        GameController.OnMusicVolumeUpdate -= new GameController.MusicVolumeUpdate(ChangeVolume);
    }
}
