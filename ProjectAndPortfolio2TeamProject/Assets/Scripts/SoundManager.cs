using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] string _volumeParameter = "SFX Volume";
    [SerializeField] AudioMixer _mixer;
    [SerializeField] float _multiplier = 30f;
    [SerializeField] Slider SFXSlider;

    private void Start()
    {
        // Load volume from PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat(_volumeParameter, 1.0f);
        SFXSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(_volumeParameter, SFXSlider.value);
    }

    private void Awake()
    {
        SFXSlider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        SetVolume(value);
    }

    private void SetVolume(float value)
    {
        if (value <= 0f)
        {
            _mixer.SetFloat(_volumeParameter, -80f); // -80 is mute
        }
        else
        {
            _mixer.SetFloat(_volumeParameter, Mathf.Log10(value) * _multiplier);
        }
    }
}
