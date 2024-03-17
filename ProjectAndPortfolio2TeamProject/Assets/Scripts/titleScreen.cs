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

    IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    IEnumerator PracticeAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void play()
    {
        StartCoroutine(PlayAfterDelay());
    }

    public void quit()
    {
        StartCoroutine(QuitAfterDelay());
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

    public void practice()
    {
        StartCoroutine(PracticeAfterDelay());
    }
}
