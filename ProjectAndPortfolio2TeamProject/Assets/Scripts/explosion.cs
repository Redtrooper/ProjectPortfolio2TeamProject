using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int damageAmount;
    [SerializeField] int pushForce;
    [SerializeField] GameObject explosionParticleFX;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(.15f);
        Destroy(gameObject);
        if (explosionParticleFX)
        {
            if (gameManager.instance.canSpawnExplosionFX())
            {
                gameManager.instance.reportExplosion();
                Instantiate(explosionParticleFX, transform.position, new Quaternion(0, 0, 0, 0)); 
            }
        }
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
