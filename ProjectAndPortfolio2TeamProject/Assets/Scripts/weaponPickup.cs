using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [Header("----- Weapon Data -----")]
    [SerializeField] weaponStats weapon;
    [SerializeField] Transform weaponExitPoint;
    [SerializeField] bool isBlank;
    [SerializeField] MeshFilter weaponModelMeshFilter;
    [SerializeField] MeshRenderer weaponModelMeshRenderer;

    private bool playerInRange;

    private void Start()
    {
        if (weaponExitPoint)
        {
            weapon.weaponExitPointPos = weaponExitPoint.localPosition; 
        }
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Pickup"))
            {
                gameManager.instance.playerScript.addNewWeapon(weapon);
                Destroy(gameObject);
            } 
        }
    }

    public void setWeaponData(weaponStats weaponToSet, Transform weaponExitPointToSet)
    {
        weapon = weaponToSet;
        weaponExitPoint = weaponExitPointToSet;
        weaponModelMeshFilter.sharedMesh = weapon.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        weaponModelMeshRenderer.sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    public void givePlayerWeapon()
    {
        weapon.weaponExitPointPos = weaponExitPoint.localPosition;
        gameManager.instance.playerScript.addNewWeapon(weapon);
    }
}
