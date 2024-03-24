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
        // load colume set slider
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        musicVolumeSlider.value = savedMusicVolume;
        soundManager.SetMusicVolume(savedMusicVolume);
    }

    public void SetMusicVolume(float volume)
    {
        // set vol in sound manage
        soundManager.SetMusicVolume(volume);

        // save setting
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }
}
