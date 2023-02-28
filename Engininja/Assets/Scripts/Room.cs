using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomTransition
{
    public Room toRoom;
    public GameObject interactableGameObject;
    public float cost;
}

[System.Serializable]
public class Room : MonoBehaviour
{
    public Collider2D coll;
    public RoomTransition[] roomTransitions;

    public static float CalcCost(Vector2 a, Vector2 b)
    {
        float absX = Mathf.Abs(a.x - b.x);
        float absY = Mathf.Abs(a.y - b.y);
        return absX + absY;
    }

    private void Start()
    {
        coll = GetComponent<Collider2D>();
        for (int i = 0; i < roomTransitions.Length; i++)
        {
            float absX = Mathf.Abs(roomTransitions[i].interactableGameObject.transform.position.x - transform.position.x);
            float absY = Mathf.Abs(roomTransitions[i].interactableGameObject.transform.position.y - transform.position.y);
            roomTransitions[i].cost = absX + absY;
        }
    }
}