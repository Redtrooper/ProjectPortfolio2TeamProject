using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] musicSounds;
    public Sound[] SFXSounds;

    public AudioSource musicSource;
    public AudioSource SFXSource;

    private const string MusicVolumeKey = "MusicVolume";

    private void Start()
    {
        PlayMusic("MusicThemes");
    }


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

        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        SetMusicVolume(savedMusicVolume);

    
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void PlayMusic(string musicTheme)
    {
        Sound music = FindMusicThemeByName(musicTheme);

        if (music == null || music.clips.Length == 0)
        {
            Debug.LogWarning("no music found or empty " + musicTheme); // debug here 
            return;
        }

        AudioClip clip = music.clips[Random.Range(0, music.clips.Length)];
        musicSource.clip = clip;
        musicSource.volume = music.volume;
        musicSource.loop = true;
        musicSource.Play();
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
    


