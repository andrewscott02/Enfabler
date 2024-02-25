using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBounds : MonoBehaviour
{
    public PCGRoom room;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ROOMBOUNDS - Trigger Enter: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            //Debug.Log("ROOMBOUNDS - Collided with player");
            DungeonGenerator.instance.CullRooms(room);
        }
        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("ROOMBOUNDS - Collided with enemy");
            if (!room.cullObjects.Contains(other.gameObject))
            {
                room.cullObjects.Add(other.gameObject);
                room.ForceAddEnemyToRoom();
                //other.gameObject.transform.SetParent(room.transform, true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("ROOMBOUNDS - Collided with enemy");
            if (room.cullObjects.Contains(other.gameObject))
            {
                room.cullObjects.Remove(other.gameObject);
                room.ForceRemoveEnemyFromRoom(null);
            }
        }
    }
}
