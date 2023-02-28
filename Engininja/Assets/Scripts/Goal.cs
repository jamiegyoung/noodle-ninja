using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class Goal : MonoBehaviour
{
    public GameObject player;
    private PlayableDirector dir;
    public AudioSource globalAudioSource;
    private bool volumeDownFlag = false;
    void Start()
    {
        dir = GetComponent<PlayableDirector>();
        dir.time = 0;
    }

    private void FixedUpdate()
    {
        if (volumeDownFlag)
        {
            globalAudioSource.volume = globalAudioSource.volume - 0.01f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            volumeDownFlag = true;
            dir.Play();
        }
    }
}
