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
        if (soundManager)
            soundManager.SetMusicVolume(savedMusicVolume); 
    }

    public void SetMusicVolume(float volume)
    {
        volume = musicVolumeSlider.value;
        soundManager.SetMusicVolume(volume);

        if (volume > 0 && !soundManager.musicSource.isPlaying)
        {
            soundManager.PlayMusic("MusicThemes");
        }

        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

}