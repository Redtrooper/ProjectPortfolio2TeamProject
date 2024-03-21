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
            Debug.Log("ATtack anim trigger");
            anim.SetTrigger("Attack");
            AttackPlayer();
        }
        else if (isAggro && !isDying)
        {
            EngageTarget();
        }
    }

    protected override void RotateTorwards()
    {
        if (gameManager.instance.player != null && !isDying)
        {
            Vector3 playerDirection = gameManager.instance.player.transform.position - transform.position;
            playerDirection.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);

            transform.rotation = targetRotation;
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

            if (!isShooting)
            {



                if (anim)
                    anim.SetTrigger("Attack");


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
