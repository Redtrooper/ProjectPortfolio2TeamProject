using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] musicSounds;
    public Sound[] SFXSounds;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public float musicVolumeChangeDuration = 1.0f;
    public float sfxVolumeChangeDuration = 0.5f;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private bool musicStarted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        // Set default volume values
        float defaultMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float defaultSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        musicVolumeSlider.value = defaultMusicVolume;
        sfxVolumeSlider.value = defaultSFXVolume;
        SetMusicVolume(defaultMusicVolume);
        SetSFXVolume(defaultSFXVolume);
    }

    private void Start()
    {
        if (musicVolumeSlider.value > 0f)
            PlayMusic("MusicThemes");
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);

        if (!musicStarted && volume > 0f)
        {
            PlayMusic("MusicThemes");
            musicStarted = true;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }

    public void PlayMusic(string musicTheme)
    {
        Sound music = FindMusicThemeByName(musicTheme);

        if (music == null || music.clips.Length == 0)
        {
            return;
        }

        AudioClip clip = music.clips[Random.Range(0, music.clips.Length)];
        musicSource.clip = clip;
        musicSource.loop = true;

        if (music.volume > 0f)
        {
            musicSource.volume = music.volume;
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }

    private Sound FindMusicThemeByName(string musicTheme)
    {
        foreach (Sound music in musicSounds)
        {
            if (music.name == musicTheme)
            {
                return music;
            }
        }
        return null;
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        sfxSource.PlayOneShot(clip, volume);
    }
}
