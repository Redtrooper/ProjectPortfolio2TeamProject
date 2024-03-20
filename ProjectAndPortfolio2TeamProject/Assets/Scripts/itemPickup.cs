using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemPickup : MonoBehaviour
{
    public itemStats item;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.addItem(item);
            Destroy(gameObject);
        }
    }

    public int getItemRarity()
    {
        return (int)item.itemRarity;
    }
}
