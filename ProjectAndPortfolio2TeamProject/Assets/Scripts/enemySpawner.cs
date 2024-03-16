using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] bool getsDisabledOnUse = true;
    [SerializeField] List<GameObject> enemyList = new List<GameObject>();
    private bool playerInRange = false;
    [SerializeField] Renderer model;
    [SerializeField] Transform enemySpawnPos;
    private Color originalColor;
    private bool isOpen;

    private void Start()
    {
        originalColor = model.material.color;
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (!isOpen && Input.GetButtonDown("Pickup"))
            {
                StartCoroutine(flashInteract());
                int randomEnemy = Random.Range(0, enemyList.Count);
                Instantiate(enemyList[randomEnemy], enemySpawnPos.position, transform.rotation);
                if (getsDisabledOnUse)
                    isOpen = true;
            }
        }
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

    IEnumerator flashInteract()
    {
        model.material.color = Color.green;
        yield return new WaitForSeconds(.15f);
        if (getsDisabledOnUse)
            model.material.color = Color.black;
        else
            model.material.color = originalColor;
    }
}
