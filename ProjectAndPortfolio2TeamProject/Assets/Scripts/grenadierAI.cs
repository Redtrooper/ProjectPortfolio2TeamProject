using System.Collections;
using UnityEngine;

public class grenadierAI : enemyAI
{
    protected override void CreateBullet()
    {
        base.CreateBullet();

        GameObject grenadeObject = Instantiate(enemyProjectile, enemyExitPoint.transform.position, Quaternion.identity);

        Vector3 directionToPlayer = (gameManager.instance.player.transform.position - transform.position).normalized;

        Rigidbody grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();
        if (grenadeRigidbody != null)
        {
            grenadeRigidbody.velocity = directionToPlayer * 10f; 
        }
    }
}
