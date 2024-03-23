using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int spawnTimer;
    [SerializeField] Transform[] spawnPos;


    int spawnCount;
    bool isSpawning;
    bool startSpawning;
    int currentPositionArrayPos = 0;
    int currentObjectArrayPos = 0;

    void Update()
    {
        if (startSpawning && !isSpawning && spawnCount < numToSpawn)
        {
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {
        isSpawning = true;

        Instantiate(objectToSpawn[currentObjectArrayPos], spawnPos[currentPositionArrayPos].transform.position, spawnPos[currentPositionArrayPos].rotation);
        if (currentPositionArrayPos != spawnPos.Count() - 1)
            currentPositionArrayPos++;
        else
            currentPositionArrayPos = 0;
        if (currentObjectArrayPos != objectToSpawn.Count() - 1)
            currentObjectArrayPos++;
        else
            currentObjectArrayPos = 0;
        spawnCount++;
        yield return new WaitForSeconds(spawnTimer);
        isSpawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
