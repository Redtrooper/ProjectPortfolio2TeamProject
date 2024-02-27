using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [Header("----- Weapon Data -----")]
    [SerializeField] weaponStats weapon;
    [SerializeField] Transform weaponExitPoint;

    private void Start()
    {
        weapon.weaponExitPointPos = weaponExitPoint.localPosition;
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
