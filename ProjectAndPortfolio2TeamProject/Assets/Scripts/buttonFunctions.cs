using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void nextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCount)
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpaused();
    }

    public void quit()
    {
        Application.Quit();
    }

    public void respawnPlayer()
    {
        gameManager.instance.playerScript.respawn();
        resume();
    }

}