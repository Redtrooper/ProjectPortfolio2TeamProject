using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class optionsMenu : MonoBehaviour
{
    [SerializeField] Slider mouseSensitivity;
    [SerializeField] TMP_Text mouseSensitivityLabel;
    [SerializeField] Toggle invertY;
    [SerializeField] Toggle firstTimeItemUI;
    [SerializeField] Animator animController;
    [SerializeField] Slider musicVolumeSlider; 

    // When the options menu opens set all the fields in it according to player prefs
    private void Start()
    {
        if (PlayerPrefs.HasKey("Mouse Sensitivity"))
            mouseSensitivity.value = PlayerPrefs.GetInt("Mouse Sensitivity");
        else
            mouseSensitivity.value = mouseSensitivity.minValue;
        if (PlayerPrefs.HasKey("Invert Y"))
        {
            int isEnabled = PlayerPrefs.GetInt("Invert Y");
            if (isEnabled == 1)
                invertY.isOn = true;
            else
                invertY.isOn = false;
        }
        else
            invertY.isOn = false;
        if(PlayerPrefs.HasKey("First Time Item UI"))
        {
            int isEnabled = PlayerPrefs.GetInt("First Time Item UI");
            if (isEnabled == 1)
                firstTimeItemUI.isOn = true;
            else
                firstTimeItemUI.isOn = false;
        }
        else
            firstTimeItemUI.isOn = true;

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicVolumeSlider.value = musicVolume;
        }
        else
        {
            musicVolumeSlider.value = 1f;
        }
    }

    public void Apply()
    {
        StartCoroutine(ApplyOnDelay());
    }

    public void Cancel()
    {
        StartCoroutine(CancelOnDelay());
    }

    private IEnumerator ApplyOnDelay()
    {
        float timer = 0f;
        float delay = 0.5f;

        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetInt("Mouse Sensitivity", (int) mouseSensitivity.value);
        PlayerPrefs.SetInt("Invert Y", invertY.isOn ? 1 : 0);
        PlayerPrefs.SetInt("First Time Item UI", firstTimeItemUI.isOn ? 1 : 0);
        GetComponentInParent<menuControls>().freezeInput = false;
        if (gameManager.instance != null)
            gameManager.instance.loadSettings();
        animController.SetTrigger("Close");
        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private IEnumerator CancelOnDelay()
    {
        float timer = 0f;
        float delay = 0.5f;
        Start();
        GetComponentInParent<menuControls>().freezeInput = false;
        animController.SetTrigger("Close");
        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public void updateMouseSensitivityLabel()
    {
        mouseSensitivityLabel.text = mouseSensitivity.value.ToString();
    }

    public void OnMusicVolumeChanged(float volume)
    {
        SoundManager.instance.SetMusicVolume(volume);
    }
}
