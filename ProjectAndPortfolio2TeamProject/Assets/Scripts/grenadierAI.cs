using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class grenadierAI : enemyAI
{
    [Header("----- Grenadier -----")]
    [SerializeField] private float grenadeSpeed = 10f;

    protected override void FireProjectile()
    {
        base.FireProjectile();

        GameObject grenadeObject = Instantiate(enemyProjectile, enemyExitPoint.transform.position, Quaternion.identity);

        if (gameManager.instance.player != null && !isDying)
        {
            Vector3 directionToPlayer = (gameManager.instance.player.transform.position - transform.position).normalized;

            Rigidbody grenadeRigidbody = grenadeObject.GetComponent<Rigidbody>();
            if (grenadeRigidbody != null)
            {
                grenadeRigidbody.velocity = directionToPlayer * grenadeSpeed;
            }
        }

        Collider grenadierCollider = GetComponent<Collider>();
        Collider grenadeCollider = grenadeObject.GetComponent<Collider>();
        if (grenadierCollider != null && grenadeCollider != null)
        {
            Physics.IgnoreCollision(grenadeCollider, grenadierCollider);
        }
    }
}
