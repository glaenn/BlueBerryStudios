using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlacedSoundFX : MonoBehaviour
{
    private AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        GameController.OnSoundVolumeUpdate += new GameController.SoundVolumeUpdate(ChangeVolume);
        audioSource = GetComponent<AudioSource>();
        ChangeVolume(GameController.instance.SoundVolume);
    }

    // Update is called once per frame
    void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
    }


}
