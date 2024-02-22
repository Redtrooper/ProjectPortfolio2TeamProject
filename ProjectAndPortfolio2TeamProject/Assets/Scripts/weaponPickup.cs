using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [SerializeField] weaponStats weapon;
    [SerializeField] Transform exitPoint;

    private void Start()
    {
        weapon.weaponExitPointPos = exitPoint.localPosition;
    }
    private void Update()
    {
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.addNewWeapon(weapon);
            Destroy(gameObject);
        }
    }
}
