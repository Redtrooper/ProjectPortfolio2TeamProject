using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    [SerializeField] string _volumeParameter = "SFX Volume";
    [SerializeField] AudioMixer _mixer;
    [SerializeField] float _multiplier = 30f;

    [SerializeField] Slider SFXSlider;

   

    private void Awake()
    {
        SFXSlider.onValueChanged.AddListener(HandleSliderValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        _mixer.SetFloat(_volumeParameter, value:Mathf.Log10(value) * _multiplier) ;
    }


}
