using UnityEngine;
using UnityEngine.UI;

public class SFXSettings : MonoBehaviour
{
    public Slider sfxVolumeSlider;
    public SoundManager soundManager;

    private const string SFXVolumeKey = "SFXVolume";

    void Start()
    {
        float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        sfxVolumeSlider.value = savedSFXVolume;
        soundManager.SetSFXVolume(savedSFXVolume);
    }

    public void OnSFXVolumeChanged(float volume)
    { 
        volume = sfxVolumeSlider.value;

        soundManager.SetSFXVolume(volume);

        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }
}
