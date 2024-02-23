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
    }
}
