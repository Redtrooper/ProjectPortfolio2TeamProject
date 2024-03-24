using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    [Header("----- Weapon Data -----")]
    [SerializeField] weaponStats weapon;
    [SerializeField] GameObject weaponExitPoint;
    [SerializeField] bool isBlank;
    [SerializeField] GameObject parentObject;
    [SerializeField] GameObject interactUI;

    private bool playerInRange;

    private void Start()
    {

        if (weaponExitPoint)
        {
            weapon.weaponExitPointPos = weaponExitPoint.transform.localPosition; 
        }
        setWeaponModel();

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
        weaponExitPoint.transform.localPosition = weaponExitPointToSet.localPosition;
        setWeaponModel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI)
                interactUI.SetActive(false);
        }
    }

    public void givePlayerWeapon()
    {
        weapon.weaponExitPointPos = weaponExitPoint.transform.localPosition;
        gameManager.instance.playerScript.addNewWeapon(weapon);
    }

    private void setWeaponModel()
    {
        MeshFilter[] childFilters = weapon.weaponModel.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter meshfilter in childFilters)
        {
            GameObject blankMesh = Instantiate(gameManager.instance.emptyMesh, parentObject.transform);
            blankMesh.transform.localPosition = meshfilter.transform.localPosition;
            blankMesh.GetComponent<MeshFilter>().sharedMesh = meshfilter.sharedMesh;
            blankMesh.GetComponent<MeshRenderer>().sharedMaterial = weapon.weaponModel.GetComponent<MeshRenderer>().sharedMaterial;
        }
    }
}
