using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public static Dictionary<Vector2Int, RoomGenerator> rooms = new();
    public static Vector2Int currentPosition = new(0, 0);

    private Vector2Int position = new(0, 0);
    public float roomSize;
    public GameObject roomPrefab;

    public GameObject doorPlusZ;
    public GameObject doorMinusZ;
    public GameObject doorPlusX;
    public GameObject doorMinusX;

    readonly Vector2Int[] directions = {
        new(1, 0),
        new(-1, 0),
        new(0, 1),
        new(0, -1)
    };

    public void Initialize(Vector2Int position)
    {
        this.position = position;
    }

    public Vector3 GetWorldSpacePosition(Vector2Int position)
    {
        return new Vector3(position.x, 0, position.y) * roomSize;
    }

    public void GenerateNewRoom(Vector2Int direction)
    {
        GameObject room = Instantiate(roomPrefab, GetWorldSpacePosition(position + direction), Quaternion.identity);
        rooms.Add(position + direction, room.GetComponent<RoomGenerator>());
    }

    public void GoToRoom(Vector2Int direction)
    {
        Vector2Int newPos = position + direction;

        // Generate the room if it does not exist
        //waow i just figured out how to write a comment  much woah  very cool
        //i am messing up the code right now beep bop boop
        if (!rooms.ContainsKey(newPos))
            GenerateNewRoom(direction);

        if (direction == directions[0])
        {
            Destroy(doorPlusX);
            Destroy(rooms[newPos].doorMinusX);
        }
        else if (direction == directions[1])
        {
            Destroy(doorMinusX);
            Destroy(rooms[newPos].doorPlusX);
        }
        else if (direction == directions[2])
        {
            Destroy(doorPlusZ);
            Destroy(rooms[newPos].doorMinusZ);
        }
        else if (direction == directions[3])
        {
            Destroy(doorMinusZ);
            Destroy(rooms[newPos].doorPlusZ);
        }

        rooms[newPos].Initialize(newPos);
    }
}
