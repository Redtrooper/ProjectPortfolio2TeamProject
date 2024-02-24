using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int damageAmount;
    [SerializeField] int pushForce;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(.15f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }


        if (other.TryGetComponent<IDamage>(out IDamage damageable))
        {
            damageable.takeDamage(damageAmount);
        }
        if(other.TryGetComponent<IPhysics>(out IPhysics pushable))
        {
            pushable.pushInDirection((other.transform.position - transform.position).normalized * pushForce);
        }
    }

}
