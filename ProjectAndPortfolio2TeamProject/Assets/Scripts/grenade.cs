using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] int speed;
    [SerializeField] float yVel;
    [SerializeField] int destroyTime;

    [SerializeField] SphereCollider explosion;
    void Start()
    {
        rb.velocity = (transform.forward + new Vector3(0, yVel, 0)) * speed;

        StartCoroutine(destroyObject());
    }

    // Update is called once per frsame
    void Update()
    {

    }

    IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(destroyTime);
        if (explosion)
        {
            Instantiate(explosion, transform.position, transform.rotation);
        }
        Destroy(gameObject);

    }

}
