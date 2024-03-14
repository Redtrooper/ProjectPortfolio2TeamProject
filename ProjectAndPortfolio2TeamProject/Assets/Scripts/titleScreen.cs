using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class titleScreen : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsScreen;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void play()
    {
        SceneManager.LoadScene(1);
    }

    public void quit()
    {
        Application.Quit();
    }

    public void openOptionsMenu()
    {
        optionsMenu.SetActive(true);
        this.GetComponent<menuControls>().freezeInput = true;
    }

    public void showCredits()
    {
        creditsScreen.SetActive(true);
        this.GetComponent<menuControls>().freezeInput = true;
    }

    public void closeCredits()
    {
        creditsScreen.SetActive(false);
        this.GetComponent<menuControls>().freezeInput = false;
    }
}
