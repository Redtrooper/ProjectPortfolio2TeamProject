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

        if (IsPlayerInRange(meleeRange) && canAttack && !isDying)
        {
            Debug.Log("Attack anim trigger");
            anim.SetTrigger("Attack");
            
        }
        else if (isAggro && !isDying)
        {
            EngageTarget();
        }
    }

    protected override void EngageTarget()
    {
        if (gameManager.instance.player != null && !isDying)
        {
            enemyAgent.stoppingDistance = desiredDistance;
            float animSpeed = enemyAgent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans));

            enemyAgent.SetDestination(gameManager.instance.player.transform.position);
            StopCoroutine(Roam());

            if (enemyAgent.remainingDistance <= enemyAgent.stoppingDistance)
            {
                RotateTowardsPlayer(); // Call the method to rotate towards the player
            }
        }
    }



    private void AttackPlayer()
    {
        if (!isDying)
        {
            int damageDealt = meleeDamage;
            gameManager.instance.playerScript.takeDamage(damageDealt);

            canAttack = false;
            Invoke("ResetAttack", attackCooldownDuration);
        }
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
