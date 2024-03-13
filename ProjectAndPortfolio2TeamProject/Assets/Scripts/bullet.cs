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
        bulletRigidBody.velocity = transform.forward * bulletSpeed;
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
            dmg.takeDamage(bulletDamageAmount);
        }

        Destroy(gameObject);
    }
}
