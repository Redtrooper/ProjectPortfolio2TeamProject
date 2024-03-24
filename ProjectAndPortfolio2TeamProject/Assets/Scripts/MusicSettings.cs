using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSettings : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public SoundManager soundManager;

    private const string MusicVolumeKey = "MusicVolume";

    void Start()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicVolumeSlider.value = savedMusicVolume;
        soundManager.SetMusicVolume(savedMusicVolume);
    }

    public void SetMusicVolume(float volume)
    {
        // Update the volume parameter with the value from the slider
        volume = musicVolumeSlider.value;

        // Set the volume to the SoundManager instance
        soundManager.SetMusicVolume(volume);

        if (volume > 0 && !soundManager.musicSource.isPlaying)
        {
            soundManager.PlayMusic("MusicThemes");
        }

        // Save the setting
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

}