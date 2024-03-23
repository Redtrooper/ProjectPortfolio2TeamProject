using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public Sound[] musicSounds; // Array of music sounds
    public Sound[] SFXSounds; // Array of SFX sounds

<<<<<<< Updated upstream
    private void Start()
    {
        // Load volume from PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat(_volumeParameter, 1.0f);
        SFXSlider.value = (float) System.Math.Truncate(savedVolume * 100)/100;
        SFXLabel.text = SFXSlider.value.ToString();
        SetVolume(savedVolume);
    }
=======
    public AudioSource musicSource; // AudioSource for music
    public AudioSource SFXSource; // AudioSource for SFX
>>>>>>> Stashed changes


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        SFXSource = gameObject.AddComponent<AudioSource>();

        // Play music on start if available
        if (musicSounds.Length > 0)
        {
            Sound music = musicSounds[Random.Range(0, musicSounds.Length)];
            AudioClip clip = music.clips[Random.Range(0, music.clips.Length)];
            musicSource.clip = clip;
            musicSource.volume = music.volume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }


    void Start()
    {
        // Initialize AudioSource if not set
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        if (SFXSource == null)
            SFXSource = gameObject.AddComponent<AudioSource>();
    }

    // Play a random music sound
    public void PlayRandomMusic()
    {
        if (musicSounds.Length > 0)
        {
            Sound music = musicSounds[Random.Range(0, musicSounds.Length)];
            AudioClip clip = music.clips[Random.Range(0, music.clips.Length)];
            musicSource.PlayOneShot(clip, music.volume);
        }
        else
        {
            Debug.LogWarning("No music clips found in the array.");
        }
    }

    // Play a random SFX sound
    public void PlayRandomSFX()
    {
        if (SFXSounds.Length > 0)
        {
            Sound SFX = SFXSounds[Random.Range(0, SFXSounds.Length)];
            AudioClip clip = SFX.clips[Random.Range(0, SFX.clips.Length)];
            SFXSource.PlayOneShot(clip, SFX.volume);
        }
        else
        {
            Debug.LogWarning("No SFX clips found in the array.");
        }
    }
}
