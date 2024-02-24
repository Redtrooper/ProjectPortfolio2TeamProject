using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    // GameManager Instance
    public static gameManager instance;

    // Game State
    private bool isPaused;
    private float timeScale;

    [Header("----- Player -----")]
    public GameObject player;
    public PlayerController playerScript;
    public GameObject playerSpawn;
    public GameObject playerDamageFlash;

    [Header("----- Menus -----")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject lossScreen;
    private GameObject activeMenu;

    [Header("----- HUD Elements -----")]
    [SerializeField] Image healthBar;
    [SerializeField] Image staminaBar;
    [SerializeField] Image progressBar;
    [SerializeField] TMP_Text currentAmmo;
    [SerializeField] TMP_Text maxAmmo;
    [SerializeField] Image ammoLineFill;
    [SerializeField] Image ammoLineBorder;
    [SerializeField] TMP_Text reloadText;

    // Private HUD Variables
    private int origHP;
    private int origStamina;
    private Color staminaBarColor;

    [Header("----- Keys -----")]
    [SerializeField] TMP_Text keyCount;
    public GameObject keyPickup;

    // Game Goal
    private int checkPointsLeft = 0;
    private int totalCheckpoints;

    void Awake()
    {
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawn = GameObject.FindWithTag("Player Spawn Position");
        origHP = playerScript.getHP();
        origStamina = playerScript.getMaxStamina();
        staminaBarColor = staminaBar.color;
        progressBar.fillAmount = 0;
        maxAmmo.text = playerScript.getMaxAmmo().ToString();
        currentAmmo.text = maxAmmo.text;
        reloadText.enabled = false;
        toggleAmmunitionUI(false);
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
        healthBar.fillAmount = (float)newHP / origHP;
    }

    public void updateStaminaBar(int newStamina)
    {
        staminaBar.fillAmount = (float)newStamina / origStamina;
    }

    public void toggleExhaustedStaminaBar()
    {
        if (staminaBar.color == staminaBarColor)
            staminaBar.color = Color.red;
        else
            staminaBar.color = staminaBarColor;
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

    public void youLose()
    {
        statePaused();
        activeMenu = lossScreen;
        activeMenu.SetActive(true);
        confineCursor();
    }

    public void updateKeyCountUI(int amount)
    {
        keyCount.text = "x" + amount.ToString();
    }

    public void updateAmmoCountUI(int amount)
    {
        currentAmmo.text = amount.ToString("00");
        maxAmmo.text = playerScript.getMaxAmmo().ToString("00");
    }

    public void toggleAmmunitionUI(bool toggle)
    {
        currentAmmo.enabled = toggle;
        maxAmmo.enabled = toggle;
        ammoLineBorder.enabled = toggle;
        ammoLineFill.enabled = toggle;
    }

    public void toggleReloadIcon()
    {
        reloadText.enabled = !reloadText.enabled;
    }
}
