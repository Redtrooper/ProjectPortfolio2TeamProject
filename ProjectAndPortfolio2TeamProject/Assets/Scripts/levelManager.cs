using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class levelManager : MonoBehaviour
{
    // Instance
    public static levelManager instance;

    // Room Prefabs to Generate From
    public List<GameObject> roomPrefabs = new List<GameObject>();

    // Room Type Lists (A,B,C,D)
    private List<List<GameObject>> roomLists;
    private List<GameObject> roomAPrefabs = new List<GameObject>();
    private List<GameObject> roomBPrefabs = new List<GameObject>();
    private List<GameObject> roomCPrefabs = new List<GameObject>();
    private List<GameObject> roomDPrefabs = new List<GameObject>();
    
    // Number of rooms that will be generated.
    [SerializeField] int roomsToGenerate;

    // Room Positions t/f
    public int currentPosX = 25;
    public int currentPosZ = 25;
    public bool[,] roomPositions = new bool[50, 50];

    // Makes sure the first room can only have 1 door
    public bool firstRoomGenerated = false;
    private bool roomsRegenerated = false;

    // List of generated rooms for room regeneration
    public List<GameObject> generatedRooms = new List<GameObject>();

    private void Start()
    {
        // Set the instance of the singleton
        instance = this;

        // Set all bools in the room positions array to false to indicate that no rooms are present at any position
        for (int i = 0; i < 50; i++)
        {
            for(int j = 0; j < 50; j++)
            {
                roomPositions[i, j] = false;
            }
        }

        // Add the room prefabs to their corresponding categories
        foreach (GameObject gameRoom in roomPrefabs)
        {
            room room = gameRoom.GetComponent<room>();
            if (room.doorAOpen)
                roomAPrefabs.Add(gameRoom);
            if (room.doorBOpen)
                roomBPrefabs.Add(gameRoom);
            if (room.doorCOpen)
                roomCPrefabs.Add(gameRoom);
            if (room.doorDOpen)
                roomDPrefabs.Add(gameRoom);
        }

        // Put those lists together for use during room generation
        roomLists  = new List<List<GameObject>>() { roomAPrefabs, roomBPrefabs, roomCPrefabs, roomDPrefabs };

        // Generate the first room for the level, allowing any type of room to be made.
        generateRoom(roomType.All);
    }

    public void generateRoom(roomType listToUse)
    {
        // Check to see if any room is present at the passed in position
        // Also check to make sure that roomsToGenerate is not less than or equal to zero, meaning no more rooms should generate.
        if (roomsToGenerate > 0 && !roomPositions[currentPosX, currentPosZ])
        {
            // Set the bool for the given position to true to indicate that there is a room there.
            roomPositions[currentPosX, currentPosZ] = true;
            roomsToGenerate--;

            // Get a random room of the given type
            GameObject roomToGenerate;
            int roomIndex = 0;
            if((int) listToUse == 0)
            {
                roomIndex = Random.Range(0, roomPrefabs.Count - 1);
                Debug.Log(roomIndex + " " + roomPrefabs.Count);
                roomToGenerate = roomPrefabs[roomIndex];
            }
            else
            {
                roomIndex = Random.Range(0, roomLists[(int)listToUse - 1].Count - 1);
                roomToGenerate = roomLists[(int)listToUse - 1][roomIndex];
            }

            // Actually make the room 
            // posX - 25 because we want the room to appear starting at 0,0 and can't use negatives in the array values so 25,25 is our "origin"
            // then multiply by the room size, in this case '50'
            generatedRooms.Add(Instantiate(roomToGenerate, new Vector3((currentPosX - 25) * 50, 0, (currentPosZ - 25) * 50), this.transform.rotation));

            if (firstRoomGenerated)
                firstRoomGenerated = false;


        }
        else if (roomsToGenerate == 0 && !roomsRegenerated)
        {
            foreach (GameObject roomObject in generatedRooms.ToList<GameObject>())
                roomObject.GetComponent<room>().regenerateRoom();
            roomsRegenerated = true;
        }
    }

    public List<bool> getNeighbors(int posX, int posZ)
    {
        List<bool> neighborsList = new List<bool>() { false, false, false, false };
        if (posX != 0 && posZ != 0)
        {
            neighborsList[0] = (roomPositions[posX - 1, posZ]);
            neighborsList[1] = (roomPositions[posX, posZ + 1]);
            neighborsList[2] = (roomPositions[posX + 1, posZ]);
            neighborsList[3] = (roomPositions[posX, posZ - 1]);
        }
        return neighborsList;
    }
}
public enum roomType 
{ 
    All,
    A,
    B,
    C,
    D
}
