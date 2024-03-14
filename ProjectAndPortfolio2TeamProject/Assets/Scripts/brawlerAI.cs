using System.Collections;
using UnityEngine;

public class brawlerAI : enemyAI
{
    [Header("----- Brawler -----")]
    [SerializeField] private float meleeRange = 2f;
    [SerializeField] private int meleeDamage = 10;
    [SerializeField] private float desiredDistance = 2.5f;
    [SerializeField] private float attackCooldownDuration = 1.5f;
    private bool canAttack = true;

    protected override void Update()
    {
        base.Update();

        if (IsPlayerInRange(meleeRange) && canAttack)
        {
            AttackPlayer();
        }
        else if (isAggro)
        {
            MoveNShoot();
        }
    }

    protected override void RotateTorwards()
    {
        if (gameManager.instance.player != null)
        {
            Vector3 playerDirection = gameManager.instance.player.transform.position - transform.position;
            playerDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);

            transform.rotation = targetRotation;
        }
    }

    protected override IEnumerator Shoot()
    {
        isShooting = true;

        // Ill add animation here for hitting 

        isShooting = false;

        yield return null;
    }

    protected override void MoveNShoot()
    {
        if (gameManager.instance.player != null)
        {
            enemyAgent.stoppingDistance = desiredDistance;

            enemyAgent.SetDestination(gameManager.instance.player.transform.position);
            StopCoroutine(Roam());

            if (!isShooting)
            {
                if (hasAnimator)
                {
                    anim.SetTrigger("Shoot");
                }

                if (DetectPlayer())
                {
                    StartCoroutine(Shoot());
                }
            }

            if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                RotateTorwards();
            }
        }
    }

    private void AttackPlayer()
    {
        int damageDealt = meleeDamage;
        gameManager.instance.playerScript.takeDamage(damageDealt);
        Debug.Log("Brawler dealt " + damageDealt + " damage to the player.");

        canAttack = false;
        Invoke("ResetAttack", attackCooldownDuration);
    }

    private void ResetAttack()
    {
        canAttack = true;
        isAggro = true;
    }

    private bool IsPlayerInRange(float range)
    {
        if (gameManager.instance.player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, gameManager.instance.player.transform.position);
            return distanceToPlayer <= range;
        }
        return false;
    }
}
