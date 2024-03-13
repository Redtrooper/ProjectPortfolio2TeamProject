using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    public void showCredits()
    {
        creditsScreen.SetActive(true);
    }

    public void closeCredits()
    {
        creditsScreen.SetActive(false);
    }


}
