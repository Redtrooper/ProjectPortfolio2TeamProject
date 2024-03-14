using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage, IHeal, IPhysics
{
    [Header("----- Player Stats -----")]
    [SerializeField] int playerHP;
    [SerializeField] float playerWalkSpeed;
    [SerializeField] float playerSprintSpeed;
    [SerializeField] int playerJumpMax;
    [SerializeField] float playerJumpForce;
    [SerializeField] float playerGravity;
    [SerializeField] int playerPushBackResolution;
    public Vector3 playerPushBack;
    private float playerCritChance = 0;
    private bool playerCanCrit = false;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] playerSteps;
    [Range(0, 1)][SerializeField] float playerStepsVol;
    [SerializeField] AudioClip[] soundHurt;
    [Range(0, 1)][SerializeField] float soundHurtVol;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] reloadSound;
    [Range(0, 1)][SerializeField] float reloadSoundVol;
    [SerializeField] AudioClip[] shootSound;
    [Range(0, 1)][SerializeField] float shootSoundVol;

    bool isPlayerSteps;

    [Header("----- Player Model & Transform -----")]
    [SerializeField] CharacterController playerController;
    [SerializeField] Transform playerModel;

    // Private Player Variables
    private int playerOrigHP;
    private int playerKeys = 0;
    private int playerJumpCount;

    // Player Movement
    private Vector3 playerMove;
    private Vector3 playerVel;

    [Header("----- Weapons -----")]
    [SerializeField] GameObject playerWeaponModel;
    [SerializeField] Transform playerExitLocation;
    [SerializeField] GameObject playerGrenade;
    [SerializeField] float playerGrenadeCooldown;
    [SerializeField] int playerGrenadeCount;
    [SerializeField] GameObject playerMuzzleFlash;
    [SerializeField] GameObject playerWeaponPickupBlank;

    // Private Weapon Variables
    private int playerMaxAmmo;
    private int playerCurrentAmmo;
    private float playerReloadTime;
    private float playerFireRate;
    private int playerWeaponKnockback;
    private int playerSelectedWeapon;
    private List<weaponStats> playerWeaponList = new List<weaponStats>();
    private GameObject playerProjectile = null;
    private float playerCurrentGrenadeCooldown = 0;
    private int playerMaxGrenades;
    private bool playerBulletsChase = false;

    // Multipliers
    public float playerDamageMultiplier = 1;
    public float playerProjectileSpeedMultiplier = 1;
    public float playerLifeStealMultiplier = 1;
    private float playerFireRateMultiplier = 1;
    private float playerWeaponKnockbackMultiplier = 1;
    private float playerHealthRegenMultiplier = 1;
    private float playerAirDashSpeedMultiplier = 1;

    [Header("----- Stamina -----")]
    [SerializeField] int playerMaxStamina;
    [SerializeField] int playerCurrentStamina;
    [SerializeField] int playerStaminaRecoveryRate;
    [SerializeField] float playerStaminaRecoveryDelay;
    [SerializeField] int playerJumpStaminaCost;

    // Player States
    private bool isShooting = false;
    private bool isReloading = false;
    private bool isExhausted = false;
    private bool isSprinting = false;
    private bool isRegeneratingStamina = false;
    private bool isRegeneratingHealth = false;
    private bool isAirDashing = false;

    // HP Regeneration
    [SerializeField] int playerHPRecoveryRate;
    [SerializeField] float playerHPRecoveryDelay;
    [SerializeField] float playerDamageRegenDelay;
    private float playerOrigDamageRegenDelay;

    // LifeSteal/Regeneration
    private bool playerCanRegenerate = true;
    public bool playerCanLifeSteal = false;
    public float playerLifeStealPercentage;

    // AirDash
    [SerializeField] float playerAirDashSpeed;
    private bool canAirDash = false;

    void Start()
    {
        playerOrigHP = playerHP;
        respawn();
        playerCurrentAmmo = playerMaxAmmo;
        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);
        playerOrigDamageRegenDelay = playerDamageRegenDelay;
        playerDamageRegenDelay = 0;
        playerMaxGrenades = playerGrenadeCount;
        playerCurrentGrenadeCooldown = playerGrenadeCooldown;
    }

    void Update()
    {
        if (!gameManager.instance.isPaused)
        {
            Movement();
            HandleSprintInput();
            ThrowGrenade();
            if (playerWeaponList.Count > 0)
            {
                selectWeapon();
                Shoot();
            }
            if (playerDamageRegenDelay > 0)
                playerDamageRegenDelay -= Time.deltaTime;
            else if (playerCanRegenerate && playerHP < playerOrigHP && !isRegeneratingHealth && playerDamageRegenDelay <= 0)
                StartCoroutine(healthRegen());
        }
    }

    public void pushInDirection(Vector3 dir)
    {
        playerPushBack += dir;
    }
    void Movement()
    {
        playerPushBack = Vector3.Lerp(playerPushBack, Vector3.zero, Time.deltaTime * playerPushBackResolution);

        if (playerController.isGrounded)
        {
            playerVel = Vector3.zero;
            playerJumpCount = 0;
            isAirDashing = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
            Crouch();
        else if (Input.GetKeyUp(KeyCode.LeftControl))
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
        playerController.Move((playerVel + playerPushBack) * Time.deltaTime);

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

        // running audio
        if (playerController.isGrounded && playerMove.normalized.magnitude > 0.3f && !isPlayerSteps)
        {
            StartCoroutine(playFootSteps());
        }

        if (!isAirDashing && canAirDash && !playerController.isGrounded && Input.GetButtonDown("Air Dash"))
        {
            airDash();
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

    IEnumerator healthRegen()
    {
        isRegeneratingHealth = true;
        playerHP += Mathf.RoundToInt(playerHPRecoveryRate * playerHealthRegenMultiplier);
        UpdateHealthBar();
        yield return new WaitForSeconds(playerHPRecoveryDelay);
        isRegeneratingHealth = false;
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
        aud.PlayOneShot(soundHurt[Random.Range(0, soundHurt.Length)], soundHurtVol);
        UpdateHealthBar();
        StartCoroutine(flashDamage());

        if (playerHP <= 0)
            gameManager.instance.youLose();

        playerDamageRegenDelay = playerOrigDamageRegenDelay;
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
        if (playerCritChance >= Random.value)
            playerCanCrit = true;
        Instantiate(playerProjectile, playerExitLocation.position, Camera.main.transform.rotation);
        playerMuzzleFlash.SetActive(true);
        playerMuzzleFlash.transform.Rotate(0, 0, Random.Range(0, 180));
        playerPushBack -= Camera.main.transform.forward * (playerWeaponKnockback * playerWeaponKnockbackMultiplier);
        playerCurrentAmmo -= 1;
        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);
        yield return new WaitForSeconds(playerFireRate * playerFireRateMultiplier);
        playerMuzzleFlash.SetActive(false);
        isShooting = false;
    }

    void PlayShootSound()
    {
        aud.PlayOneShot(shootSound[Random.Range(0, shootSound.Length)], shootSoundVol);
    }

    void PlayReloadSound()
    {
        aud.PlayOneShot(reloadSound[Random.Range(0, reloadSound.Length)], reloadSoundVol);
    }

    void Shoot()
    {

        if (Input.GetButton("Shoot") && !isShooting && playerCurrentAmmo > 0 && !isReloading)
        {
            StartCoroutine(ShootTimer());
            PlayShootSound();
        }
        else if (Input.GetButtonDown("Reload") && !isReloading && playerCurrentAmmo < playerMaxAmmo)
        {
            isReloading = true;
            gameManager.instance.toggleReloadIcon();
            Invoke("Reload", playerReloadTime);
            PlayReloadSound();
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
        playerController.height /= 2;
    }
    void UnCrouch()
    {
        playerController.height *= 2;
    }

    public void respawn()
    {
        playerPushBack = Vector3.zero;
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
        if (playerWeaponList.Count < 2)
        {
            playerWeaponList.Add(weapon);
        }
        else
        {
            GameObject weaponPickup = Instantiate(playerWeaponPickupBlank, transform.position + (Vector3.Scale(Camera.main.transform.forward.normalized, new Vector3(1,0,1)) * 3), transform.rotation);
            weaponPickup.GetComponent<weaponPickup>().setWeaponData(playerWeaponList[playerSelectedWeapon], playerExitLocation);
            playerWeaponList.RemoveAt(playerSelectedWeapon);
            playerWeaponList.Insert(playerSelectedWeapon, weapon);
        }
        gameManager.instance.toggleAmmunitionUI(weapon.weaponTakesAmmo);
        playerSelectedWeapon = playerWeaponList.Count - 1;

        playerFireRate = weapon.weaponFireRate;
        playerProjectile = weapon.weaponProjectile;
        playerCurrentAmmo = weapon.weaponAmmoCurr;
        playerMaxAmmo = weapon.weaponAmmoMax;
        playerReloadTime = weapon.weaponReloadTime;
        playerWeaponKnockback = weapon.weaponKnockback;
        playerExitLocation.localPosition = weapon.weaponExitPointPos;

        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);

        playerWeaponModel.GetComponent<MeshFilter>().sharedMesh = weapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        playerWeaponModel.GetComponent<MeshRenderer>().sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void changeWeapon()
    {
        gameManager.instance.toggleAmmunitionUI(playerWeaponList[playerSelectedWeapon].weaponTakesAmmo);
        playerFireRate = playerWeaponList[playerSelectedWeapon].weaponFireRate;
        playerProjectile = playerWeaponList[playerSelectedWeapon].weaponProjectile;

        playerCurrentAmmo = playerWeaponList[playerSelectedWeapon].weaponAmmoCurr;
        playerMaxAmmo = playerWeaponList[playerSelectedWeapon].weaponAmmoMax;
        playerReloadTime = playerWeaponList[playerSelectedWeapon].weaponReloadTime;
        playerWeaponKnockback = playerWeaponList[playerSelectedWeapon].weaponKnockback;
        playerExitLocation.localPosition = playerWeaponList[playerSelectedWeapon].weaponExitPointPos;

        gameManager.instance.updateAmmoCountUI(playerCurrentAmmo);

        playerWeaponModel.GetComponent<MeshFilter>().sharedMesh = playerWeaponList[playerSelectedWeapon].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        playerWeaponModel.GetComponent<MeshRenderer>().sharedMaterial = playerWeaponList[playerSelectedWeapon].weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectWeapon()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && playerSelectedWeapon < playerWeaponList.Count - 1)
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

    void ThrowGrenade()
    {
        if(playerGrenadeCount < playerMaxGrenades)
        {
            playerCurrentGrenadeCooldown -= Time.deltaTime;
            if(playerCurrentGrenadeCooldown <= 0)
            {
                playerCurrentGrenadeCooldown = playerGrenadeCooldown;
                playerGrenadeCount += 1;
                gameManager.instance.updateGrenadeCountUI(playerGrenadeCount);
            }
        }
        if (Input.GetButtonDown("Grenade") && playerGrenadeCount > 0)
        {
            Instantiate(playerGrenade, transform.position + new Vector3(0, 1, 0), Camera.main.transform.rotation);
            playerGrenadeCount -= 1;
            gameManager.instance.updateGrenadeCountUI(playerGrenadeCount);
        }
    }

    IEnumerator playFootSteps()
    {
        isPlayerSteps = true;
        aud.PlayOneShot(playerSteps[Random.Range(0, playerSteps.Length)], playerStepsVol);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else if (isSprinting)
            yield return new WaitForSeconds(0.3f);

        isPlayerSteps = false;

    }

    public void addItem(itemStats item)
    {
        playerHP = (int)Mathf.Round(playerHP * item.healthMultiplier);
        playerWalkSpeed *= item.speedMultiplier;
        playerSprintSpeed *= item.speedMultiplier;
        playerJumpMax += item.extraJumps;
        playerJumpForce *= item.jumpForceMultiplier;
        playerMaxStamina = (int)Mathf.Round(playerMaxStamina * item.maxStaminaMultiplier);
        playerStaminaRecoveryRate = (int)Mathf.Round(playerStaminaRecoveryRate * item.staminaRecoveryRateMultiplier);

        playerDamageMultiplier *= item.damageMultiplier;
        playerProjectileSpeedMultiplier *= item.bulletSpeedMultiplier;
        playerFireRateMultiplier *= item.fireRateMultiplier;
        playerWeaponKnockbackMultiplier *= item.weaponRecoilMultiplier;

        if (item.lifeSteal)
        {
            playerCanLifeSteal = true;
            playerCanRegenerate = false;
            playerLifeStealMultiplier *= item.lifeStealMultiplier;
        }
        if (!playerCanLifeSteal)
            playerHealthRegenMultiplier *= item.healthRegenerationMultiplier;
        if (playerCritChance == 0 && item.critChanceMultiplier > 0)
            playerCritChance = item.critChanceMultiplier - 1;
        else
            playerCritChance *= item.critChanceMultiplier;

        if (item.grenadeCount > 0)
        {
            playerGrenadeCount += item.grenadeCount;
            playerMaxGrenades += item.grenadeCount;
            gameManager.instance.updateGrenadeCountUI(playerGrenadeCount); 
        }

        if (item.airDash && !canAirDash)
            canAirDash = true;
        else
            playerAirDashSpeedMultiplier *= item.airDashSpeedMultiplier;

        if (item.bulletChase)
            playerBulletsChase = true;
    }

    public bool canPlayerCrit()
    {
        if(playerCanCrit)
        {
            playerCanCrit = false;
            return true;
        }
        else
            return false;
    }

    private void airDash()
    {
        isAirDashing = true;
        playerVel = (Camera.main.transform.forward * playerAirDashSpeed) * playerAirDashSpeedMultiplier;
    }

    public bool canBulletChase()
    {
        return playerBulletsChase;
    }
}