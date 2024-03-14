using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class itemSpawner : MonoBehaviour
{
    [SerializeField] List<itemPickup> itemList = new List<itemPickup>();
    private bool playerInRange = false;
    [SerializeField] Renderer model;
    [SerializeField] Transform itemSpawnPos;
    private Color originalColor;
    private bool isOpen;
    private List<itemPickup> commonItemList = new List<itemPickup>();
    private List<itemPickup> rareItemList = new List<itemPickup>();
    [SerializeField] float rareDropChance;
    private List<itemPickup> legendaryItemList = new List<itemPickup>();
    [SerializeField] float legendaryDropChance;
    private List<itemPickup> wackyItemList = new List<itemPickup>();
    [SerializeField] float wackyDropChance;

    private void Start()
    {
        originalColor = model.material.color;
        foreach(itemPickup item in itemList)
        {
            switch (item.getItemRarity())
            {
                case 0:
                    commonItemList.Add(item);
                    break;
                case 1:
                    rareItemList.Add(item);
                    break;
                case 2:
                    legendaryItemList.Add(item);
                    break;
                case 3:
                    wackyItemList.Add(item);
                    break;

            }
        }
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (!isOpen && Input.GetButtonDown("Pickup"))
            {
                for (int i = 0; i < 100; i++)
                {
                    StartCoroutine(flashInteract());
                    Instantiate(getRandomItem(), itemSpawnPos.position, transform.rotation);
                    // isOpen = true; 
                }
            }
        }
    }

    private itemPickup getRandomItem()
    {
        itemPickup itemToReturn = null;
        float randomDropChance = Random.value;
        int randomItemIndex;

        if (randomDropChance <= wackyDropChance)
        {
            // Roll Wacky Items
            Debug.Log("Wacky Spawn");
            randomItemIndex = Random.Range(0, wackyItemList.Count);
            itemToReturn = wackyItemList[randomItemIndex];
        }
        else if (randomDropChance <= legendaryDropChance)
        {
            // Roll Legendary Items
            Debug.Log("Legendary Spawn");
            randomItemIndex = Random.Range(0, legendaryItemList.Count);
            itemToReturn = legendaryItemList[randomItemIndex];
        }
        else if (randomDropChance <= rareDropChance)
        {
            // Roll Rare Items
            Debug.Log("Rare Spawn");
            randomItemIndex = Random.Range(0, rareItemList.Count);
            itemToReturn = rareItemList[randomItemIndex];
        }
        else if (randomDropChance > rareDropChance)
        {
            // Roll Common Items
            Debug.Log("Common Spawn");
            randomItemIndex = Random.Range(0, commonItemList.Count);
            itemToReturn = commonItemList[randomItemIndex];
        }

        return itemToReturn;
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
        model.material.color = Color.black;
    }
}