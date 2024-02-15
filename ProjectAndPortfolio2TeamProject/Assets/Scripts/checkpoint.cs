using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    void Start()
    {
        gameManager.instance.updateGameGoal(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.updateGameGoal(-1);
            gameManager.instance.playerSpawn.transform.position = gameObject.transform.position;
            Destroy(gameObject);
        }
    }
}
