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
    }

    public void Apply()
    {
        PlayerPrefs.SetInt("Mouse Sensitivity", (int) mouseSensitivity.value);
        PlayerPrefs.SetInt("Invert Y", invertY.isOn ? 1 : 0);
        PlayerPrefs.SetInt("First Time Item UI", firstTimeItemUI.isOn ? 1 : 0);
        gameObject.SetActive(false);
        GetComponentInParent<menuControls>().freezeInput = false;
        if (gameManager.instance != null)
            gameManager.instance.loadSettings();
    }

    public void Cancel()
    {
        Start();
        gameObject.SetActive(false);
        GetComponentInParent<menuControls>().freezeInput = false;
    }

    public void updateMouseSensitivityLabel()
    {
        mouseSensitivityLabel.text = mouseSensitivity.value.ToString();
    }
}
