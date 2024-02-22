using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IHeal
{
    [Header("----- Player Stats -----")]
    [SerializeField] int HP;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;

    [Header("----- Player Model & Transform -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Transform playerModel;

    // Private Player Variables
    private Vector3 originalPlayerScale;
    private int origHP;
    private int keys = 0;
    private int jumpCount;

    // Player Movement
    private Vector3 move;
    private Vector3 playerVel;

    [Header("----- Weapons -----")]
    [SerializeField] GameObject weaponModel;
    [SerializeField] Transform exitlocation;

    // Private Weapon Variables
    private int maxAmmo;
    private int currentAmmo;
    private float reloadTime;
    private float firerate;
    private Vector3 originalGunScale;
    private int selectedWeapon;
    private List<weaponStats> weaponList = new List<weaponStats>();
    private GameObject projectile = null;

    [Header("----- Stamina -----")]
    [SerializeField] int maxStamina;
    [SerializeField] int currentStamina;
    [SerializeField] int staminaRecoveryRate;
    [SerializeField] float staminaRecoveryDelay;
    [SerializeField] int jumpStaminaCost;

    // Crouch Scaling Stuff
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);

    // Player States
    private bool isShooting = false;
    private bool isReloading = false;
    private bool isExhausted = false;
    private bool isSprinting = false;
    private bool isRegeneratingStamina = false;

    void Start()
    {
        origHP = HP;
        respawn();
        originalPlayerScale = playerModel.localScale;
        originalGunScale = weaponModel.transform.localScale;
        currentAmmo = maxAmmo;
        gameManager.instance.updateAmmoCountUI(currentAmmo);
    }

    void Update()
    {
        Movement();
        HandleSprintInput();
        if (weaponList.Count > 0)
        {
            selectWeapon();
            Shoot(); 
        }
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
        }

       if (Input.GetKey(KeyCode.LeftControl))
            Crouch();
       else
            UnCrouch();

        float speed = isSprinting ? sprintSpeed : walkSpeed;
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        move = (horizontalInput * transform.right + verticalInput * transform.forward).normalized;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && currentStamina > 0 && !isExhausted)
        {
            UseStamina(jumpStaminaCost);
            playerVel.y = jumpForce;
            jumpCount++;
        }


        playerVel.y += gravity * Time.deltaTime;
        controller.Move(playerVel * Time.deltaTime);

        if (isSprinting)
        {
            int staminaCost = Mathf.CeilToInt(speed * Time.deltaTime);
            UseStamina(staminaCost);
        }
        else
        {
            if (currentStamina < maxStamina && !isRegeneratingStamina)
            {
                StartCoroutine(staminaRegen());
            }
        }

    }

    void HandleSprintInput()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && !isExhausted; 
    }

    [ContextMenu("Use Max Stamina")]
    private void UseMaxStamina()
    {
        UseStamina(maxStamina);
    }

    IEnumerator staminaRegen()
    {
        isRegeneratingStamina = true;
        currentStamina += staminaRecoveryRate;
        UpdateStaminaBar();
        if (isExhausted && currentStamina == maxStamina) 
        {
            isExhausted = false;
            gameManager.instance.toggleExhaustedStaminaBar();
        }
        yield return new WaitForSeconds(staminaRecoveryDelay);
        isRegeneratingStamina = false;
    }

    public void UseStamina(int amount)
    {
        if (currentStamina - amount >= 0)
        {
            currentStamina -= amount;
            UpdateStaminaBar();
            if (currentStamina <= 0)
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
        HP -= amount;
        UpdateHealthBar();
        StartCoroutine(flashDamage());

        if (HP <= 0)
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
        if (HP < origHP)
        {
            if (HP + amount <= origHP)
            {
                HP += amount;
                UpdateHealthBar();  
            }
            else
            {
                HP = origHP;
                UpdateHealthBar();
            }
        }
    }

    public int getHP()
    {
        return HP;
    }

    public int getMaxStamina()
    {
        return maxStamina;
    }

    void UpdateStaminaBar()
    {
        if (gameManager.instance != null)
            gameManager.instance.updateStaminaBar(currentStamina);
    }

    void UpdateHealthBar()
    {
        if (gameManager.instance != null)
            gameManager.instance.updateHealthBar(HP);
    }

    IEnumerator ShootTimer()
    {
        isShooting = true;
        Instantiate(projectile, exitlocation.position, Camera.main.transform.rotation);
        currentAmmo -= 1;
        gameManager.instance.updateAmmoCountUI(currentAmmo);
        yield return new WaitForSeconds(firerate);
        isShooting = false;
    }
    void Shoot()
    {

        if (Input.GetButton("Shoot") && !isShooting && currentAmmo > 0 && !isReloading)
            StartCoroutine(ShootTimer());
        else if (Input.GetButtonDown("Reload") && !isReloading && currentAmmo < maxAmmo)
        {
            isReloading = true;
            gameManager.instance.toggleReloadIcon();
            Invoke("Reload", reloadTime);
        }
    }

    void Reload()
    {
        currentAmmo = maxAmmo;
        gameManager.instance.updateAmmoCountUI(currentAmmo);
        gameManager.instance.toggleReloadIcon();
        isReloading = false;
    }
    void Crouch()
    {
        playerModel.localScale = crouchScale;
        Camera.main.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        // this line here makes the gun scale stay the same when crouching - john
        weaponModel.transform.localScale = Vector3.Scale(originalGunScale, new Vector3(1f, 1f / crouchScale.y, 1f));

    }
    void UnCrouch()
    {
        playerModel.localScale = playerScale;
        Camera.main.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        weaponModel.transform.localScale = originalGunScale; 
    }


        public void respawn()
    {
        HP = origHP;
        UpdateHealthBar();
        currentStamina = maxStamina;
        UpdateStaminaBar();
        if (isExhausted)
        {
            isExhausted = false;
            gameManager.instance.toggleExhaustedStaminaBar();
        }
        

        if (gameManager.instance.playerSpawn != null)
        {
            controller.enabled = false;
            transform.position = gameManager.instance.playerSpawn.transform.position;
            controller.enabled = true; 
        }
    }

    public int getKeyCount()
    {
        return keys;
    }

    public void giveKey(int amount)
    {
        keys += amount;
        gameManager.instance.updateKeyCountUI(keys);
    }

    public void useKey(int amount)
    {
        keys -= amount;
        gameManager.instance.updateKeyCountUI(keys);
    }

    public int getMaxAmmo()
    {
        return maxAmmo;
    }

    public void addNewWeapon(weaponStats weapon)
    {
        selectedWeapon = weaponList.Count - 1;

        weaponList.Add(weapon);

        firerate = weapon.weaponFireRate;
        projectile = weapon.weaponProjectile;
        currentAmmo = weapon.weaponAmmoCurr;
        maxAmmo = weapon.weaponAmmoMax;
        reloadTime = weapon.weaponReloadTime;
        exitlocation.localPosition = weapon.weaponExitPointPos;


        gameManager.instance.updateAmmoCountUI(currentAmmo);

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void changeWeapon()
    {
        firerate = weaponList[selectedWeapon].weaponFireRate;
        projectile = weaponList[selectedWeapon].weaponProjectile;
        currentAmmo = weaponList[selectedWeapon].weaponAmmoCurr;
        maxAmmo = weaponList[selectedWeapon].weaponAmmoMax;
        reloadTime = weaponList[selectedWeapon].weaponReloadTime;
        exitlocation.localPosition = weaponList[selectedWeapon].weaponExitPointPos;

        gameManager.instance.updateAmmoCountUI(currentAmmo);

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weaponList[selectedWeapon].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weaponList[selectedWeapon].weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectWeapon()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedWeapon < weaponList.Count - 1)
        {
            selectedWeapon++;
            changeWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon > 0)
        {
            selectedWeapon--;
            changeWeapon();
        }
    }
  
}