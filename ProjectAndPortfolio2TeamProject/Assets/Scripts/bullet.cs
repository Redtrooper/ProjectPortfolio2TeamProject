using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    [SerializeField] bool sourceIsFriendly;
    
    void Start()
    {
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.isTrigger)
            return;
        else if (other.CompareTag("Player") && sourceIsFriendly)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null )
        {
            dmg.takeDamage(damageAmount);
        }

        Destroy(gameObject);
    }
    
    
}
