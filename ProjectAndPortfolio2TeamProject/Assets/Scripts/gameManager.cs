using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class gameManager : MonoBehaviour
{
    // GameManager Instance
    public static gameManager instance;

    // Game State
    public bool isPaused;
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
    public GameObject optionsMenu;
    private GameObject activeMenu;

    [Header("----- HUD Elements -----")]
    [SerializeField] Image healthBar;
    [SerializeField] Image staminaBar;
    [SerializeField] List<Image> checkPointImages;
    [SerializeField] TMP_Text currentAmmo;
    [SerializeField] TMP_Text maxAmmo;
    [SerializeField] Image ammoLineFill;
    [SerializeField] Image ammoLineBorder;
    [SerializeField] TMP_Text reloadText;

    // Private HUD Variables
    private int maxHP;
    private int maxStamina;
    private Color staminaBarColor;
    private int checkpointInitial;
    private int currentCheckpoint;

    [Header("----- Keys -----")]
    [SerializeField] TMP_Text keyCount;
    public GameObject keyPickup;

    [Header("----- Grenade -----")]
    [SerializeField] TMP_Text grenadeCount;

    // Game Goal
    private int checkPointsLeft = 0;

    // Enemy List
    List<Transform> enemyTransforms = new List<Transform>();

    // Weapons List
    public List<weaponPickup> playerWeapons = new List<weaponPickup>();

    // Player Stat Loading
    public bool playerShouldLoadStats = false;

    // Empty Mesh For Gun Models
    public GameObject emptyMesh;

    [Header("----- Item UI -----")]
    [SerializeField] Canvas itemCanvas;
    [SerializeField] GameObject emptyItemUI;
    [SerializeField] Canvas newItemUICanvas;
    [SerializeField] GameObject newItemUI;
    public List<itemPickup> itemsList = new List<itemPickup>();
    private List<GameObject> itemUIs = new List<GameObject>();
    private int differentItemsCollected;
    private GameObject currentNewItemUI = null;
    private bool useFirstTimeItemUI = false;

    [Header("----- Framerate Optimization -----")]
    [SerializeField] int maxExplosionFX;
    private int currentExplosionFX;

    // Control Screen
    [SerializeField] GameObject controlScreen;
    [SerializeField] GameObject webGLControlScreen;

    void Awake()
    {
        if (PlayerPrefs.HasKey("Player ShouldLoadStats"))
        {
            playerShouldLoadStats = PlayerPrefs.GetInt("Player ShouldLoadStats") == 1 ? true : false;
        }
        instance = this;
        timeScale = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        playerSpawn = GameObject.FindWithTag("Player Spawn Position");
        if (playerShouldLoadStats)
        { 
            loadUIData();
        }
        else
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                webGLControlScreen.SetActive(true);
            }
            else
            {
                controlScreen.SetActive(true);
            }
        }

        maxHP = playerScript.getHP();
        maxStamina = playerScript.getMaxStamina();
        staminaBarColor = staminaBar.color;
        maxAmmo.text = playerScript.getMaxAmmo().ToString();
        currentAmmo.text = maxAmmo.text;
        reloadText.enabled = false;
        toggleAmmunitionUI(false);
        foreach(Image checkPoint in checkPointImages)
        {
            checkPoint.enabled = false;
        }
        loadSettings();
        giveTextOutline(keyCount);
        giveTextOutline(grenadeCount);
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if ((Input.GetButtonDown("P")) && !isPaused)
            {
                statePaused();
                activeMenu = pauseMenu;
                activeMenu.SetActive(isPaused);
                confineCursor();
            } 
        }
        else
        {
            if ((Input.GetButtonDown("Cancel") && !isPaused))
            {
                statePaused();
                activeMenu = pauseMenu;
                activeMenu.SetActive(isPaused);
                confineCursor();
            }
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
        healthBar.fillAmount = (float)newHP / maxHP;
    }

    public void updateHealthBarMax(int currentPlayerHP,int newMaxHP)
    {
        maxHP = newMaxHP;
        updateHealthBar(currentPlayerHP);
    }

    public void updateStaminaBar(int newStamina)
    {
        staminaBar.fillAmount = (float)newStamina / maxStamina;
    }

    public void updateStaminaBarMax(int currentPlayerStamina, int newMaxStamina)
    {
        maxStamina = newMaxStamina;
        updateStaminaBar(currentPlayerStamina);
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
        { 
            checkPointImages[checkpointInitial].enabled = true;
            checkpointInitial++;
        }
        else if(amount < 0)
        {
            checkPointImages[currentCheckpoint].color = new Color(0, 255, 255);
            currentCheckpoint++;
        }

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
        keyCount.text = "x" + amount;
    }

    public void updateAmmoCountUI(int amount)
    {
        currentAmmo.text = amount.ToString("00");
        maxAmmo.text = playerScript.getMaxAmmo().ToString("00");
        giveTextOutline(currentAmmo);
        giveTextOutline(maxAmmo);
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

    public void loadSettings()
    {
        if (PlayerPrefs.HasKey("Mouse Sensitivity"))
            Camera.main.GetComponent<CameraController>().cameraSensitivity = PlayerPrefs.GetInt("Mouse Sensitivity");
        if(PlayerPrefs.HasKey("Invert Y"))
        {
            int isEnabled = PlayerPrefs.GetInt("Invert Y");
            if (isEnabled == 1)
                Camera.main.GetComponent<CameraController>().cameraInvertY = true;
            else
                Camera.main.GetComponent<CameraController>().cameraInvertY = false;
        }
        if(PlayerPrefs.HasKey("First Time Item UI"))
        {
            int isEnabled = PlayerPrefs.GetInt("First Time Item UI");
            if (isEnabled == 1)
                useFirstTimeItemUI = true;
            else
                useFirstTimeItemUI = false;

        }
    }

    public void updateGrenadeCountUI(int amount)
    {
        grenadeCount.text = "x" + amount;
    }

    public void enemyReportAlive(Transform enemyTransform)
    {
        enemyTransforms.Add(enemyTransform);
    }

    public void enemyReportDead(Transform enemyTransform)
    {
        enemyTransforms.Remove(enemyTransform);
    }

    public Transform findNearestEnemy()
    {
        Transform closestEnemyTransform = null;
        float closestEnemyDist = float.MaxValue;

        foreach (Transform enemyTransform in enemyTransforms)
        {
            float distance = Vector3.Distance(player.transform.position, enemyTransform.position);
            if (distance < closestEnemyDist)
            {
                closestEnemyTransform = enemyTransform;
                closestEnemyDist = distance;
            }
        }
        
        return closestEnemyTransform;
    }

    public GameObject getActiveMenu()
    {
        return activeMenu;
    }

    private void giveTextOutline(TMP_Text textToOutline)
    {
        textToOutline.outlineColor = Color.black;
        textToOutline.outlineWidth = 0.2f;
    }

    public void updateItemUI(itemStats item)
    {
        bool itemCollected = false;
        foreach (GameObject itemUIToCheck in itemUIs)
        {
            if (itemUIToCheck.GetComponentInChildren<Image>().sprite == item.itemSprite)
            {
                itemCollected = true;
                if (item.itemName != "Grenade")
                {
                    int itemCount = ++itemUIToCheck.GetComponent<ItemUI>().itemCount;
                    itemUIToCheck.GetComponentInChildren<TMP_Text>().text = "x" + itemCount;
                }
            }
        }

        if (!itemCollected)
        {
            if (useFirstTimeItemUI)
            {
                if (currentNewItemUI)
                    Destroy(currentNewItemUI);
                currentNewItemUI = Instantiate(newItemUI, newItemUICanvas.transform);
                Image[] newItemImages = currentNewItemUI.GetComponentsInChildren<Image>();
                newItemImages[1].sprite = item.itemSprite;
                TMP_Text[] newItemText = currentNewItemUI.GetComponentsInChildren<TMP_Text>();
                newItemText[0].text = item.itemName;
                newItemText[1].text = item.itemDescription;  
            }
            if (item.itemName != "Grenade")
            {
                GameObject tempItemUI = Instantiate(emptyItemUI, itemCanvas.transform);
                itemUIs.Add(tempItemUI);
                tempItemUI.transform.localPosition += Vector3.left * (-85 * differentItemsCollected);
                tempItemUI.GetComponent<ItemUI>().item = item;
                tempItemUI.GetComponentInChildren<Image>().sprite = item.itemSprite;
                differentItemsCollected++;
            }
        }
    }

    public void saveUIData()
    {
        string itemDataToSave = "";
        foreach(GameObject itemUI in itemUIs)
        {
            for(int i = 0; i < itemsList.Count; i++)
            {
                if (itemUI.GetComponent<ItemUI>().item == itemsList[i].item)
                { 
                    itemDataToSave += (i.ToString() + '/');
                    itemDataToSave += (itemUI.GetComponent<ItemUI>().itemCount.ToString() + '.');
                }
            }
        }
        PlayerPrefs.SetString("Item UI", itemDataToSave.ToString());
    }

    public void loadUIData()
    {
        char[] itemUIToLoad = PlayerPrefs.GetString("Item UI").ToCharArray();
        string itemIndexStr = "";
        int itemIndex = 0;
        string itemCountStr = "";
        int itemCount = 0;
        char lastOperator = '.';
        for(int i = 0; i < itemUIToLoad.Length; i++)
        {
            char currentChar = itemUIToLoad[i];
            if (currentChar == '.')
            {
                if (Int32.TryParse(itemIndexStr, out itemIndex) && Int32.TryParse(itemCountStr, out itemCount))
                {
                    for (; itemCount > 0; itemCount--)
                    {
                        playerScript.addItem(itemsList[itemIndex].item);
                    }
                }
                itemIndexStr = "";
                itemCountStr = "";
                lastOperator = '.';
            }
            else if (currentChar == '/')
                lastOperator = '/';
            else if(lastOperator == '.')
                itemIndexStr += currentChar;
            else if(lastOperator == '/')
                itemCountStr += currentChar;
        }
    }

    public void reportExplosion()
    {
        currentExplosionFX++;
    }

    public bool canSpawnExplosionFX()
    {
        return currentExplosionFX < maxExplosionFX;
    }
}
