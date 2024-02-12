using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [SerializeField] CharacterController controller;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] float jumpForce;
    [SerializeField] float gravity;
    [SerializeField] int HP;

    Vector3 move;
    Vector3 playerVel;
    int jumpCount;
    bool isSprinting = false;
    StaminaBar staminaBar;
    HealthBar healthBar;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);

    void Start()
    {
        FindStaminaBar();
        FindHealthBar();
        healthBar.setMaxHealth(HP);
    }

    void Update()
    { 
        Movement();
        HandleSprintInput();
    }

    void FindStaminaBar()
    {
        staminaBar = FindObjectOfType<StaminaBar>();
    }

    void FindHealthBar()
    {
        healthBar = FindObjectOfType<HealthBar>();
    }

    void Movement()
    {
        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        }

        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }

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
            staminaBar.UseStamina(staminaCost);
        }

    }

    void HandleSprintInput()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift) && staminaBar.HasStamina();
        staminaBar.IsPlayerSprinting = isSprinting;
    }


    public void takeDamage(int amount)
    {
        HP -= amount;
        healthBar.UpdateHealthBar(HP);
    }
}