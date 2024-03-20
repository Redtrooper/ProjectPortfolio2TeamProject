using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class key
    : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.giveKey(1);
            Destroy(gameObject);
        }
    }
}
