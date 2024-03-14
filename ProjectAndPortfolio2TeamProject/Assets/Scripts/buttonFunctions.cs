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

        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime; 
            yield return null; 
        }

        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }

        resume();
    }

    public void resume()
    {
        gameManager.instance.stateUnpaused();
        gameManager.instance.lockCursor();
    }

    public void restart()
    {
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

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }



    public void quit()
    {
        StartCoroutine(QuitAfterDelay());
    }

    IEnumerator QuitAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    public void respawnPlayer()
    {
        gameManager.instance.playerScript.respawn();
        resume();
    }
}
