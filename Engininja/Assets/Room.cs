using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Room : MonoBehaviour
{
    public Vector2 prevStairsLocation;
    public Vector2 nextStairsLocation;
    public Room nextRoom;
    public Room previousRoom;
}
