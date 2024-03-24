using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class itemSpawner : MonoBehaviour
{
    [SerializeField] bool getsDisabledOnUse = true;
    private bool playerInRange = false;
    [SerializeField] Renderer model;
    [SerializeField] Transform itemSpawnPos;
    private bool isOpen;
    private List<itemPickup> commonItemList = new List<itemPickup>();
    private List<itemPickup> rareItemList = new List<itemPickup>();
    [SerializeField] float rareDropChance;
    private List<itemPickup> legendaryItemList = new List<itemPickup>();
    [SerializeField] float legendaryDropChance;
    private List<itemPickup> wackyItemList = new List<itemPickup>();
    [SerializeField] float wackyDropChance;
    [SerializeField] Animator itemSpawnerAnim;

    private void Start()
    {
        foreach(itemPickup item in gameManager.instance.itemsList)
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
                StartCoroutine(spawnRandomItem());
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
            randomItemIndex = Random.Range(0, wackyItemList.Count);
            itemToReturn = wackyItemList[randomItemIndex];
        }
        else if (randomDropChance <= legendaryDropChance)
        {
            // Roll Legendary Items
            randomItemIndex = Random.Range(0, legendaryItemList.Count);
            itemToReturn = legendaryItemList[randomItemIndex];
        }
        else if (randomDropChance <= rareDropChance)
        {
            // Roll Rare Items
            randomItemIndex = Random.Range(0, rareItemList.Count);
            itemToReturn = rareItemList[randomItemIndex];
        }
        else if (randomDropChance > rareDropChance)
        {
            // Roll Common Items
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

    private IEnumerator spawnRandomItem()
    {
        if (getsDisabledOnUse)
            isOpen = true;
        if (itemSpawnerAnim)
            itemSpawnerAnim.SetTrigger("Open");
        yield return new WaitForSeconds(1.0f);
        Instantiate(getRandomItem(), itemSpawnPos.position, transform.rotation);
    }
}
