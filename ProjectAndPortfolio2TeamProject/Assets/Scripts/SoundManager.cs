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

    private void Start()
    {
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        SetMusicVolume(savedMusicVolume);

        float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        SetSFXVolume(savedSFXVolume);

        musicVolumeSlider.value = savedMusicVolume;
        sfxVolumeSlider.value = savedSFXVolume;

        if (savedMusicVolume > 0f || musicVolumeSlider.value > 0f)
            PlayMusic("MusicThemes");
    }


    void Awake()
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
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }


    private IEnumerator ChangeMusicVolume(float volume)
    {
        float currentTime = 0;
        float startVolume = musicSource.volume;

        while (currentTime < musicVolumeChangeDuration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, volume, currentTime / musicVolumeChangeDuration);
            yield return null;
        }

        musicSource.volume = volume;
    }


    public void PlayMusic(string musicTheme)
    {
        Sound music = FindMusicThemeByName(musicTheme);

        if (music == null || music.clips.Length == 0)
        {
            Debug.LogWarning("No music" + musicTheme);
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
}