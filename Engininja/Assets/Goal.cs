using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class Goal : MonoBehaviour
{
    public GameObject player;
    private PlayableDirector dir;
    void Start()
    {
        dir = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("coll");
            dir.Play();
        }
    }
}
