using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomTransitions
{
    public Room toRoom;
    public GameObject interactableGameObject;
}

[System.Serializable]
public class Room : MonoBehaviour
{
    public RoomTransitions[] roomTransitions;
}
