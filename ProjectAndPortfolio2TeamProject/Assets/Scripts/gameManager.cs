using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    private float timeScale;

    private GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winScreen;

    [SerializeField] Image healthBar;
    [SerializeField] Image staminaBar;
    [SerializeField] Image progressBar;

    public GameObject player;
    public PlayerController playerScript;
    private int origHP;
    private int origStamina;

    public GameObject playerDamageFlash; // flash here

    int checkPointsLeft = 0;
    public int totalCheckpoints;

    public bool isPaused;
    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        origHP = playerScript.getHP();
        origStamina = playerScript.getMaxStamina();
        progressBar.fillAmount = 0;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            statePaused();
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);
            confineCursor();
        }
    }

    public void statePaused()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void stateUnpaused()
    {
        isPaused = false;
        Time.timeScale = timeScale;
        activeMenu.SetActive(false);
        activeMenu = null;
    }

    public void lockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void confineCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void updateHealthBar(int newHP)
    {
        healthBar.fillAmount = (float) newHP/origHP;
    }

    public void updateStaminaBar(int newStamina)
    {
        staminaBar.fillAmount = (float) newStamina/origStamina;
    }

    public void updateGameGoal(int amount)
    {
        checkPointsLeft += amount;
        if (amount > 0)
            totalCheckpoints += amount;
        else
            progressBar.fillAmount = (float)(totalCheckpoints - checkPointsLeft) / totalCheckpoints;

        if (checkPointsLeft == 0)
        {
            statePaused();
            activeMenu = winScreen;
            activeMenu.SetActive(true);
            confineCursor();
        }
    }
}
