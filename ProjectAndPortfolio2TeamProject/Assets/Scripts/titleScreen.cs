using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class titleScreen : MonoBehaviour
{
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text creditsText;

    IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2);
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
        titleText.outlineColor = Color.black;
        titleText.outlineWidth = 0.2f;
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
        StartCoroutine(EnableCreditsTextAfterDelay());
    }

    IEnumerator EnableCreditsTextAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        creditsText.gameObject.SetActive(true);
    }

    public void practice()
    {
        StartCoroutine(PracticeAfterDelay());
    }
}
