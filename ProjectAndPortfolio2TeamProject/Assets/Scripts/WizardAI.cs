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

        if (player != null && !isDying)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider != null)
            {
                Vector3 playerColliderPosition = playerCollider.bounds.center;
                Vector3 directionToPlayer = playerColliderPosition - enemyExitPoint.transform.position;

                directionToPlayer.Normalize();

                anim.SetTrigger("Shoot");

                GameObject bullet = Instantiate(enemyProjectile, enemyExitPoint.transform.position, Quaternion.identity);
       
                Rigidbody bulletRB = bullet.GetComponent<Rigidbody>();

                if (bulletRB != null)
                {
                    StartCoroutine(ChasePlayer(bulletRB, directionToPlayer));
                }
            }
        }
    }

    protected override void EngageTarget()
    {
        if (!isDying)
        {
            base.EngageTarget();

            float animSpeed = enemyAgent.velocity.normalized.magnitude;
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), animSpeed, Time.deltaTime * animSpeedTrans)); // walk 1 animation here ------------
        }

    }


    protected override IEnumerator Shoot()
    {
        if (!isShooting && !isDying)
        {
            isShooting = true;
            yield return new WaitForSeconds(enemyFireRate);
            CreateBullet();
            isShooting = false;
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
