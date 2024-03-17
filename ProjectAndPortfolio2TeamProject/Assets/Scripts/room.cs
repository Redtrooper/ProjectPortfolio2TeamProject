using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room : MonoBehaviour
{
    [Header("----- Open Doors -----")]
    public bool doorAOpen;
    public bool doorBOpen;
    public bool doorCOpen;
    public bool doorDOpen;

    public int posX;
    public int posZ;

    private void Start()
    {
        posX = (int)(transform.position.x/50) + 25;
        posZ = (int)(transform.position.z / 50) + 25;
        int openDoors = (doorAOpen ? 1 : 0) + (doorBOpen ? 1 : 0) + (doorCOpen ? 1 : 0) + (doorDOpen ? 1 : 0);
        if (openDoors > 1 || !levelManager.instance.firstRoomGenerated)
        {
            // If there are open doors, generate neighboring rooms on each of them.
            if (doorAOpen)
            {
                levelManager.instance.currentPosX = posX - 1;
                levelManager.instance.currentPosZ = posZ;
                levelManager.instance.generateRoom(roomType.C); 
            }
            if (doorBOpen)
            {
                levelManager.instance.currentPosX = posX;
                levelManager.instance.currentPosZ = posZ + 1;
                levelManager.instance.generateRoom(roomType.D);
            }
            if (doorCOpen)
            {
                levelManager.instance.currentPosX = posX + 1;
                levelManager.instance.currentPosZ = posZ;
                levelManager.instance.generateRoom(roomType.A);
            }
            if (doorDOpen)
            {
                levelManager.instance.currentPosX = posX;
                levelManager.instance.currentPosZ = posZ - 1;
                levelManager.instance.generateRoom(roomType.B); 
            }

        }
    }

    public void regenerateRoom()
    {
        List<bool> neighbors = levelManager.instance.getNeighbors(posX, posZ);
        doorAOpen = neighbors[0];
        doorBOpen = neighbors[1];
        doorCOpen = neighbors[2];
        doorDOpen = neighbors[3];
        GameObject roomToGenerate = null;
        foreach(GameObject roomObject in levelManager.instance.roomPrefabs)
        {
            room room = roomObject.GetComponent<room>();
            if (doorAOpen == room.doorAOpen && doorBOpen == room.doorBOpen && doorCOpen == room.doorCOpen && doorDOpen == room.doorDOpen)
            {
                roomToGenerate = roomObject;
                break;
            }

        }
        if (roomToGenerate)
        {
            Instantiate(roomToGenerate, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }
        else
            Debug.Log("Room could not be regenerated.");
    }
}

