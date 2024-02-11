using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Slider staminaBar;
    [SerializeField] private int maxStamina = 200;
    [SerializeField] private int currentStamina;
    [SerializeField] private int staminaRecoveryRate = 20;

    private bool isPlayerSprinting = false;
    public static StaminaBar instance;

    public bool IsPlayerSprinting
    {
        get { return isPlayerSprinting; }
        set { isPlayerSprinting = value; }
    }

    void Start()
    {
        currentStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;

        StartCoroutine(RecoverStamina());
    }

    [SerializeField] private float recoveryDelay = 1f;

    IEnumerator RecoverStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(recoveryDelay);
            if (!IsPlayerSprinting && currentStamina < maxStamina)
            {
                currentStamina += staminaRecoveryRate;
                staminaBar.value = currentStamina;
            }
        }
    }

    [ContextMenu("Use Max Stamina")]
    private void UseMaxStamina()
    {
        UseStamina(maxStamina);
    }

    public void UseStamina(int amount)
    {
        if (currentStamina - amount >= 0)
        {
            currentStamina -= amount;
            staminaBar.value = currentStamina;
        }
        else
        {
            Debug.Log("Not enough stamina");
        }
    }

    public bool HasStamina()
    {
        return currentStamina > 0;
    }

    public int GetCurrentStamina()
    {
        return currentStamina;
    }

    public void SetInstance()
    {
        instance = this;
    }
}

