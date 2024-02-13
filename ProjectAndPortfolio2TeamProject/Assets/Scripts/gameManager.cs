using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    private float timeScale;

    private GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;

    [SerializeField] Slider healthBar;
    [SerializeField] Slider staminaBar;

    public GameObject player;

    public bool isPaused;
    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        initializeHealthBar();
        initializeStaminaBar();
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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public int getPlayerHP()
    {
        return player.GetComponent<PlayerController>().getHP();
    }

    void initializeHealthBar()
    {
        healthBar.maxValue = player.GetComponent<PlayerController>().getHP();
        healthBar.value = healthBar.maxValue;
    }

    void initializeStaminaBar()
    {
        staminaBar.maxValue = player.GetComponent<PlayerController>().getMaxStamina();
        staminaBar.value = staminaBar.maxValue;
    }
    public void updateHealthBar(int newHP)
    {
        healthBar.value = newHP;
    }

    public void updateStaminaBar(int newStamina)
    {
        staminaBar.value = newStamina;
    }
}
