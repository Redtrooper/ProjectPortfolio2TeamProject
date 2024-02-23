using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IHeal
{
    [Header("----- Player Stats -----")]
    [SerializeField] int playerHP;
    [SerializeField] float playerWalkSpeed;
    [SerializeField] float playerSprintSpeed;
    [SerializeField] int playerJumpMax;
    [SerializeField] float playerJumpForce;
    [SerializeField] float playerGravity;

    [Header("----- Player Model & Transform -----")]
    [SerializeField] CharacterController playerController;
    [SerializeField] Transform playerModel;

    // Private Player Variables
    private Vector3 originalPlayerScale;
    private int playerOrigHP;
    private int playerKeys = 0;
    private int playerJumpCount;

    // Player Movement
    private Vector3 playerMove;
    private Vector3 playerVel;

    [Header("----- Weapons -----")]
    [SerializeField] GameObject playerWeaponModel;
    [SerializeField] Transform playerExitLocation;
    [SerializeField] GameObject grenade;

    // Private Weapon Variables
    private int playerMaxAmmo;
    private int playerCurrentAmmo;
    private float playerReloadTime;
    private float playerFireRate;
    private Vector3 playerOriginalGunScale;
    private int playerSelectedWeapon;
    private List<weaponStats> playerWeaponList = new List<weaponStats>();
    private GameObject playerProjectile = null;

    [Header("----- Stamina -----")]
    [SerializeField] int playerMaxStamina;
    [SerializeField] int playerCurrentStamina;
    [SerializeField] int playerStaminaRecoveryRate;
    [SerializeField] float playerStaminaRecoveryDelay;
    [SerializeField] int playerJumpStaminaCost;

    // Crouch Scaling Stuff
    private Vector3 playerCrouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);

    // Player States
    private bool isShooting = false;
    private bool isReloading = false;
    private bool isExhausted = false;
    private bool isSprinting = false;
    private bool isRegeneratingStamina = false;

    void Start()
    {
        playerOrigHP = playerHP;
        respawn();
        originalPlayerScale = playerModel.localScale;
        playerOriginalGunScale = playerWeaponModel.transform.localScale;
        playerCurrentAmmo = playerMaxAmmo;
        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);
    }

    void Update()
    {
        Movement();
        HandleSprintInput();
        if (playerWeaponList.Count > 0)
        {
            selectWeapon();
            Shoot(); 
        }
    }

    void Movement()
    {
        if (playerController.isGrounded)
        {
            playerVel = Vector3.zero;
            playerJumpCount = 0;
        }

       if (Input.GetKey(KeyCode.LeftControl))
            Crouch();
       else
            UnCrouch();

        float speed = isSprinting ? playerSprintSpeed : playerWalkSpeed;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        playerMove = (horizontalInput * transform.right + verticalInput * transform.forward).normalized;

        playerController.Move(playerMove * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && playerJumpCount < playerJumpMax && playerCurrentStamina > 0 && !isExhausted)
        {
            UseStamina(playerJumpStaminaCost);
            playerVel.y = playerJumpForce;
            playerJumpCount++;
        }


        playerVel.y += playerGravity * Time.deltaTime;
        playerController.Move(playerVel * Time.deltaTime);

        if (isSprinting)
        {
            int staminaCost = Mathf.CeilToInt(speed * Time.deltaTime);
            UseStamina(staminaCost);
        }
        else
        {
            if (playerCurrentStamina < playerMaxStamina && !isRegeneratingStamina)
            {
                StartCoroutine(staminaRegen());
            }
        }

    }

    void HandleSprintInput()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && playerCurrentStamina > 0 && !isExhausted; 
    }

    [ContextMenu("Use Max Stamina")]
    private void UseMaxStamina()
    {
        UseStamina(playerMaxStamina);
    }

    IEnumerator staminaRegen()
    {
        isRegeneratingStamina = true;
        playerCurrentStamina += playerStaminaRecoveryRate;
        UpdateStaminaBar();
        if (isExhausted && playerCurrentStamina == playerMaxStamina) 
        {
            isExhausted = false;
            gameManager.instance.toggleExhaustedStaminaBar();
        }
        yield return new WaitForSeconds(playerStaminaRecoveryDelay);
        isRegeneratingStamina = false;
    }

    public void UseStamina(int amount)
    {
        if (playerCurrentStamina - amount >= 0)
        {
            playerCurrentStamina -= amount;
            UpdateStaminaBar();
            if (playerCurrentStamina <= 0)
            { 
                isExhausted = true;
                gameManager.instance.toggleExhaustedStaminaBar();
            }
        }
        else
        {
            Debug.Log("Not enough stamina");
        }
    }


    public void takeDamage(int amount)
    {
        playerHP -= amount;
        UpdateHealthBar();
        StartCoroutine(flashDamage());

        if (playerHP <= 0)
            gameManager.instance.youLose();
    }

    IEnumerator flashDamage() // just like in class
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }

    public void heal(int amount)
    {
        if (playerHP < playerOrigHP)
        {
            if (playerHP + amount <= playerOrigHP)
            {
                playerHP += amount;
                UpdateHealthBar();  
            }
            else
            {
                playerHP = playerOrigHP;
                UpdateHealthBar();
            }
        }
    }

    public int getHP()
    {
        return playerHP;
    }

    public int getMaxStamina()
    {
        return playerMaxStamina;
    }

    void UpdateStaminaBar()
    {
        if (gameManager.instance != null)
            gameManager.instance.updateStaminaBar(playerCurrentStamina);
    }

    void UpdateHealthBar()
    {
        if (gameManager.instance != null)
            gameManager.instance.updateHealthBar(playerHP);
    }

    IEnumerator ShootTimer()
    {
        isShooting = true;
        Instantiate(playerProjectile, playerExitLocation.position, Camera.main.transform.rotation);
        playerCurrentAmmo -= 1;
        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);
        yield return new WaitForSeconds(playerFireRate);
        isShooting = false;
    }
    void Shoot()
    {

        if (Input.GetButton("Shoot") && !isShooting && playerCurrentAmmo > 0 && !isReloading)
            StartCoroutine(ShootTimer());
        else if (Input.GetButtonDown("Reload") && !isReloading && playerCurrentAmmo < playerMaxAmmo)
        {
            isReloading = true;
            gameManager.instance.toggleReloadIcon();
            Invoke("Reload", playerReloadTime);
        }
    }

    void Reload()
    {
        playerCurrentAmmo = playerMaxAmmo;
        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);
        gameManager.instance.toggleReloadIcon();
        isReloading = false;
    }
    void Crouch()
    {
        playerModel.localScale = playerCrouchScale;
        Camera.main.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        // this line here makes the gun scale stay the same when crouching - john
        playerWeaponModel.transform.localScale = Vector3.Scale(playerOriginalGunScale, new Vector3(1f, 1f / playerCrouchScale.y, 1f));

    }
    void UnCrouch()
    {
        playerModel.localScale = playerScale;
        Camera.main.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        playerWeaponModel.transform.localScale = playerOriginalGunScale; 
    }


        public void respawn()
    {
        playerHP = playerOrigHP;
        UpdateHealthBar();
        playerCurrentStamina = playerMaxStamina;
        UpdateStaminaBar();
        if (isExhausted)
        {
            isExhausted = false;
            gameManager.instance.toggleExhaustedStaminaBar();
        }
        

        if (gameManager.instance.playerSpawn != null)
        {
            playerController.enabled = false;
            transform.position = gameManager.instance.playerSpawn.transform.position;
            playerController.enabled = true; 
        }
    }

    public int getKeyCount()
    {
        return playerKeys;
    }

    public void giveKey(int amount)
    {
        playerKeys += amount;
        gameManager.instance.updateKeyCountUI(playerKeys);
    }

    public void useKey(int amount)
    {
        playerKeys -= amount;
        gameManager.instance.updateKeyCountUI(playerKeys);
    }

    public int getMaxAmmo()
    {
        return playerMaxAmmo;
    }

    public void addNewWeapon(weaponStats weapon)
    {
        playerSelectedWeapon = playerWeaponList.Count - 1;

        playerWeaponList.Add(weapon);

        playerFireRate = weapon.weaponFireRate;
        playerProjectile = weapon.weaponProjectile;
        playerCurrentAmmo = weapon.weaponAmmoCurr;
        playerMaxAmmo = weapon.weaponAmmoMax;
        playerReloadTime = weapon.weaponReloadTime;
        playerExitLocation.localPosition = weapon.weaponExitPointPos;


        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);

        playerWeaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        playerWeaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void changeWeapon()
    {
        playerFireRate = playerWeaponList[playerSelectedWeapon].weaponFireRate;
        playerProjectile = playerWeaponList[playerSelectedWeapon].weaponProjectile;
        playerCurrentAmmo = playerWeaponList[playerSelectedWeapon].weaponAmmoCurr;
        playerMaxAmmo = playerWeaponList[playerSelectedWeapon].weaponAmmoMax;
        playerReloadTime = playerWeaponList[playerSelectedWeapon].weaponReloadTime;
        playerExitLocation.localPosition = playerWeaponList[playerSelectedWeapon].weaponExitPointPos;

        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);

        playerWeaponModel.GetComponent<MeshFilter>().sharedMesh = playerWeaponList[playerSelectedWeapon].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        playerWeaponModel.GetComponent<MeshRenderer>().sharedMaterial = playerWeaponList[playerSelectedWeapon].weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectWeapon()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && playerSelectedWeapon < playerWeaponList.Count - 1)
        {
            playerSelectedWeapon++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && playerSelectedWeapon > 0)
        {
            playerSelectedWeapon--;
            changeWeapon();
        }
    }
  
    void throwGrenade()
    {
        if(Input.GetButton("Grenade"))
        {
            Instantiate(grenade, transform.position + new Vector3(0, 1, 0), Camera.main.transform.rotation);
        }
    }

}