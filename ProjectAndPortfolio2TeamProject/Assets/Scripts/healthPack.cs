using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class healthPack : MonoBehaviour
{
    [SerializeField] int healthGain;
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if (other.CompareTag("Player"))
        {
            IDamage dmg = other.GetComponent<IDamage>();
            int playerHP = gameManager.instance.getPlayerHP();
            if (dmg != null && playerHP < 100)
            {
                if (healthGain + playerHP > 100)
                { 
                    dmg.takeDamage(playerHP - 100);
                    Destroy(gameObject);
                }
                else
                {
                    dmg.takeDamage(-healthGain);
                    Destroy(gameObject);
                }
            }
        }
    }
}
