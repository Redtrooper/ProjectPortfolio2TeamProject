using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IHeal
{
    [SerializeField] CharacterController controller;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] int HP;
    [SerializeField] Transform playerModel;
    [SerializeField] Transform gun;
    private Vector3 originalPlayerScale;
    private Vector3 originalGunScale;

    private int maxAmmo;
    private int currentAmmo;
    private float reloadTime;
    private bool isReloading = false;
    private bool isExhausted;


    [SerializeField] int jumpStaminaCost;
    [SerializeField] int maxStamina;
    [SerializeField] int currentStamina;
    [SerializeField] int staminaRecoveryRate;
    [SerializeField] float recoveryDelay;

    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    bool isSprinting = false;
    bool staminaRegenerating = false;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);
    int origHP;
    int keys = 0;

    private float firerate = 0;
    [SerializeField] Transform exitlocation;
    private GameObject projectile = null;
    [SerializeField] GameObject weaponModel;
    bool shootcd;

    void Start()
    {
        origHP = HP;
        respawn();
        originalPlayerScale = playerModel.localScale;
        originalGunScale = gun.localScale;
        currentAmmo = maxAmmo;
        gameManager.instance.updateAmmoCountUI(currentAmmo);
    }

    void Update()
    {
        Movement();
        HandleSprintInput();
        Shoot();
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

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
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
            if (currentStamina < maxStamina && !staminaRegenerating)
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
        staminaRegenerating = true;
        currentStamina += staminaRecoveryRate;
        UpdateStaminaBar();
        if (isExhausted && currentStamina == maxStamina) 
        {
            isExhausted = false;
            gameManager.instance.toggleExhaustedStaminaBar();
        }
        yield return new WaitForSeconds(recoveryDelay);
        staminaRegenerating = false;
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
        shootcd = true;
        Instantiate(projectile, exitlocation.position, Camera.main.transform.rotation);
        currentAmmo -= 1;
        gameManager.instance.updateAmmoCountUI(currentAmmo);
        yield return new WaitForSeconds(firerate);
        shootcd = false;
    }
    void Shoot()
    {

        if (Input.GetButton("Shoot") && !shootcd && currentAmmo > 0 && !isReloading)
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
        gun.localScale = Vector3.Scale(originalGunScale, new Vector3(1f, 1f / crouchScale.y, 1f));

    }
    void UnCrouch()
    {
        playerModel.localScale = playerScale;
        Camera.main.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        gun.localScale = originalGunScale; 
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

    public void setWeaponStats(weaponStats weapon, Transform exitPoint)
    {
        firerate = weapon.weaponFireRate;
        projectile = weapon.weaponProjectile;
        currentAmmo = weapon.weaponAmmoCurr;
        maxAmmo = weapon.weaponAmmoMax;
        reloadTime = weapon.weaponReloadTime;
        exitlocation.localPosition = exitPoint.localPosition;

        gameManager.instance.updateAmmoCountUI(currentAmmo);

        weaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
  
}