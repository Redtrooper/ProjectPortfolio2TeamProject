using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [Header("----- RigidBody -----")]
    [SerializeField] Rigidbody bulletRigidBody;

    [Header("----- Bullet Properties -----")]
    public int bulletDamageAmount;
    public int bulletSpeed;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] bool bulletSourceIsFriendly;
    
    void Start()
    {
        bulletRigidBody.velocity = transform.forward * (bulletSpeed * gameManager.instance.playerScript.playerProjectileSpeedMultiplier);
        Destroy(gameObject, bulletDestroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
            return;
        else if (other.CompareTag("Player") && bulletSourceIsFriendly)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null )
        {
            int damageToApply = Mathf.RoundToInt(bulletDamageAmount * gameManager.instance.playerScript.playerDamageMultiplier);

            dmg.takeDamage(damageToApply);
            if (bulletSourceIsFriendly && gameManager.instance.playerScript.playerCanLifeSteal)
            {
                Debug.Log("DTA " + damageToApply);
                Debug.Log((damageToApply * gameManager.instance.playerScript.playerLifeStealPercentage) * gameManager.instance.playerScript.playerLifeStealMultiplier);
                gameManager.instance.playerScript.heal(Mathf.RoundToInt((damageToApply * gameManager.instance.playerScript.playerLifeStealPercentage) * gameManager.instance.playerScript.playerLifeStealMultiplier));
            }
        }

        Destroy(gameObject);
    }
}
