using System.Collections;
using System.Collections.Generic;
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

    // Stamina Values
    [SerializeField] int maxStamina = 200;
    [SerializeField] int currentStamina;
    [SerializeField] int staminaRecoveryRate = 20;
    [SerializeField] float recoveryDelay = 1f;

    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    bool isSprinting = false;
    bool staminaRegenerating = false;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);
    int origHP;

    [SerializeField] float firerate;
    [SerializeField] GameObject exitlocation;
    [SerializeField] GameObject projectile;
    bool shootcd;

    void Start()
    {
        origHP = HP;
        currentStamina = maxStamina;
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
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
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
        yield return new WaitForSeconds(recoveryDelay);
        staminaRegenerating = false;
    }

    public void UseStamina(int amount)
    {
        if (currentStamina - amount >= 0)
        {
            currentStamina -= amount;
            UpdateStaminaBar();
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
        Instantiate(projectile, exitlocation.transform.position, Camera.main.transform.rotation);
        yield return new WaitForSeconds(firerate);
        shootcd = false;
    }
    void Shoot()
    {
        if (Input.GetButton("Shoot") && !shootcd)
            StartCoroutine(ShootTimer());
    }
    void Crouch()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }
    void UnCrouch()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

  
}