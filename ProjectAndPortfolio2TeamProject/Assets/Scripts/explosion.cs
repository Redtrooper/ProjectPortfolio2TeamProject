using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int damageAmount;


    void Start()
    {
        StartCoroutine(destroyAfter());
        Destroy(gameObject);
    }

    IEnumerator destroyAfter()
    {
        yield return new WaitForSeconds(.3f);
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
    }

}
