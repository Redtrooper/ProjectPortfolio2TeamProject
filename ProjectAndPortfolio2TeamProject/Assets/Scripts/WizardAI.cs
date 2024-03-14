using System.Collections;
using UnityEngine;

public class wizardAI : enemyAI
{
    [Header("----- Wizard -----")]
    [SerializeField] private float bulletSpeed;
    private readonly float chaseUpdateInterval = 0f; // i may update this in the future 

    protected override void CreateBullet()
    {
        GameObject player = gameManager.instance.player;

        if (player != null)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider != null)
            {
                Vector3 playerColliderPosition = playerCollider.bounds.center;
                Vector3 directionToPlayer = playerColliderPosition - enemyExitPoint.transform.position;

                directionToPlayer.Normalize();

                GameObject bullet = Instantiate(enemyProjectile, enemyExitPoint.transform.position, Quaternion.identity);

                Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

                if (bulletRB != null)
                {
                    StartCoroutine(ChasePlayer(bulletRB, directionToPlayer));
                }
            }
        }
    }

    private IEnumerator ChasePlayer(Rigidbody bulletRB, Vector3 directionToPlayer)
    {

        while (true)
        {
            GameObject player = gameManager.instance.player;

            if (player != null)
            {
                Collider playerCollider = player.GetComponent<Collider>();
                if (playerCollider != null)
                {
                    Vector3 playerColliderPosition = playerCollider.bounds.center;
                    Vector3 newDirectionToPlayer = playerColliderPosition - bulletRB.position;

                    newDirectionToPlayer.Normalize();
                    bulletRB.velocity = newDirectionToPlayer * bulletSpeed;
                }
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(chaseUpdateInterval);

            if (bulletRB == null || !bulletRB.gameObject.activeInHierarchy)
            {
                break;
            }
        }
    }

}
