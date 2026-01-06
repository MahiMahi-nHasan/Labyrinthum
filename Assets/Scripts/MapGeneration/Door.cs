using UnityEngine;

public class Door : Interactable
{
    public RoomGenerator master;
    public Vector2Int direction;

    public override void Interact()
    {
        master.GoToRoom(direction);
    }
}