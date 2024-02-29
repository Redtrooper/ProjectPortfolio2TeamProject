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

    // When the options menu opens set all the fields in it according to player prefs
    private void Start()
    {
        if (PlayerPrefs.HasKey("Mouse Sensitivity"))
            mouseSensitivity.value = PlayerPrefs.GetInt("Mouse Sensitivity");
        if (PlayerPrefs.HasKey("Invert Y"))
        {
            int isEnabled = PlayerPrefs.GetInt("Invert Y");
            if (isEnabled == 1)
                invertY.isOn = true;
            else
                invertY.isOn = false;
        }
    }

    public void Apply()
    {
        if (PlayerPrefs.HasKey("Mouse Sensitivity"))
            PlayerPrefs.SetInt("Mouse Sensitivity", (int) mouseSensitivity.value);
        if (PlayerPrefs.HasKey("Invert Y"))
        {
            if (invertY.isOn)
                PlayerPrefs.SetInt("Invert Y", 1);
            else
                PlayerPrefs.SetInt("Invert Y", 0);
        }
        gameObject.SetActive(false);
    }

    public void Cancel()
    {
        Start();
        gameObject.SetActive(false);
    }

    public void updateMouseSensitivityLabel()
    {
        mouseSensitivityLabel.text = mouseSensitivity.value.ToString();
    }
}
