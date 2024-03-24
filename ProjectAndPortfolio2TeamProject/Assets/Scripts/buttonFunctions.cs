using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void nextLevel()
    {
        StartCoroutine(NextLevelAfterDelay());
    }

    IEnumerator NextLevelAfterDelay()
    {
        float timer = 0f;
        float delay = 1f;

        gameManager.instance.playerShouldLoadStats = true;
        PlayerPrefs.SetInt("Player ShouldLoadStats", gameManager.instance.playerShouldLoadStats ? 1 : 0);
        gameManager.instance.playerScript.savePlayerData();
        gameManager.instance.saveUIData();

        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (SceneManager.GetActiveScene().buildIndex == 1) 
        {
            gameManager.instance.playerShouldLoadStats = false;
            PlayerPrefs.SetInt("Player ShouldLoadStats", gameManager.instance.playerShouldLoadStats ? 1 : 0);
        }

        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(2);
        }

        resume();
    }

    public void resume()
    {
        StartCoroutine(resumeOnDelay());
    }

    IEnumerator resumeOnDelay()
    {
        float timer = 0f;
        float delay = 1f;

        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        gameManager.instance.stateUnpaused();
        gameManager.instance.lockCursor();
    }

    public void restart()
    {
        gameManager.instance.playerShouldLoadStats = false;
        PlayerPrefs.SetInt("Player ShouldLoadStats", gameManager.instance.playerShouldLoadStats ? 1 : 0);
        StartCoroutine(RestartAfterDelay());
    }

    IEnumerator RestartAfterDelay()
    {
        float timer = 0f;
        float delay = 1f; 

        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime; 
            yield return null; 
        }

        if (SceneManager.GetActiveScene().buildIndex != 1)
            SceneManager.LoadScene(2); 
        else
            SceneManager.LoadScene(1);
        gameManager.instance.stateUnpaused();
    }



    public void quit()
    {
        StartCoroutine(QuitAfterDelay());
    }

    IEnumerator QuitAfterDelay()
    {
        float timer = 0f;
        float delay = 1f;

        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void respawnPlayer()
    {
        gameManager.instance.playerScript.respawn();
        resume();
    }

    public void openOptionsMenu()
    {
        gameManager.instance.optionsMenu.SetActive(true);
        gameManager.instance.getActiveMenu().GetComponent<menuControls>().freezeInput = true;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Player ShouldLoadStats", 0);
    }
}
