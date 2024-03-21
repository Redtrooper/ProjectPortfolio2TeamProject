using System.Collections;
using UnityEngine;

public class wizardAI : enemyAI
{
    private readonly float chaseUpdateInterval = 0f; 

    protected override IEnumerator Shoot()
    {
        if (!isShooting && !isDying)
        {
            isShooting = true;
            if (anim != null)
            {
                anim.SetTrigger("Shoot");
            }
            yield return new WaitForSeconds(enemyFireRate);
            isShooting = false;
        }
    }

    private IEnumerator ChasePlayer(GameObject bullet, Vector3 directionToPlayer)
    {
        Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

        while (true)
        {
            GameObject player = gameManager.instance.player;

            if (player != null)
            {
                Collider playerCollider = player.GetComponent<Collider>();
                if (playerCollider != null)
                {
                    Vector3 playerColliderPosition = playerCollider.bounds.center;
                    Vector3 newDirectionToPlayer = playerColliderPosition - bullet.transform.position;

                    newDirectionToPlayer.Normalize();
                    bulletRB.velocity = newDirectionToPlayer * bulletSpeed;
                }
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(chaseUpdateInterval);

            if (bullet == null || !bullet.activeInHierarchy)
            {
                break;
            }
        }
    }

    protected override void FireProjectile()
    {
        if (enemyProjectile != null && !isDying)
        {
            GameObject bullet = Instantiate(enemyProjectile, enemyExitPoint.transform.position, Quaternion.identity);
            StartCoroutine(ChasePlayer(bullet, Vector3.zero));
        }
    }
}
