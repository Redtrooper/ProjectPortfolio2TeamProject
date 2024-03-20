using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [Header("----- RigidBody -----")]
    [SerializeField] Rigidbody bulletRigidBody;

    [Header("----- Bullet Properties -----")]
    [SerializeField] int bulletDamageAmount;
    [SerializeField] int bulletSpeed;
    [SerializeField] int bulletDestroyTime;
    [SerializeField] bool bulletSourceIsFriendly;
    [SerializeField] bool chasePlayer = false;

    private Transform bulletNearestEnemyTransform;

    void Start()
    {
        if (bulletSourceIsFriendly && gameManager.instance.playerScript.canPlayerCrit())
            bulletDamageAmount *= 2;
        if (bulletSourceIsFriendly)
            bulletRigidBody.velocity = transform.forward * (bulletSpeed * gameManager.instance.playerScript.playerProjectileSpeedMultiplier);
        else
            bulletRigidBody.velocity = transform.forward * bulletSpeed;
        if (bulletSourceIsFriendly && gameManager.instance.playerScript.canBulletChase())
        {
            bulletNearestEnemyTransform = gameManager.instance.findNearestEnemy();
        }
        Destroy(gameObject, bulletDestroyTime);
    }

    private void Update()
    {
        if (bulletSourceIsFriendly && gameManager.instance.playerScript.canBulletChase() && bulletNearestEnemyTransform != null)
            transform.position = Vector3.MoveTowards(transform.position, bulletNearestEnemyTransform.position, Time.deltaTime * bulletSpeed * gameManager.instance.playerScript.playerProjectileSpeedMultiplier);
        if(chasePlayer)
            transform.position = Vector3.MoveTowards(transform.position, gameManager.instance.player.transform.position, Time.deltaTime * bulletSpeed);
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
                gameManager.instance.playerScript.heal(Mathf.RoundToInt((damageToApply * gameManager.instance.playerScript.playerLifeStealPercentage) * gameManager.instance.playerScript.playerLifeStealMultiplier));
            }
        }

        Destroy(gameObject);
    }
}
