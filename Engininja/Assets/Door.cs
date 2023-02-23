using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour, Interactable
{
    private Animator anim;
    private Collider2D coll;
    private ShadowCaster2D shadowCaster;
    public bool isLocked;

    public bool IsInteractable => !isLocked;
    public AudioSource openAudio;
    public AudioSource closeAudio;

    private void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        shadowCaster = GetComponent<ShadowCaster2D>();
    }

    public void Interact()
    {
        bool isOpen = anim.GetBool("isOpen");
        set(!isOpen);
    }

    public void set(bool open)
    {
        anim.SetBool("isOpen", open);
        coll.isTrigger = open;
        shadowCaster.selfShadows = !open;
        shadowCaster.castsShadows = !open;
        if (open)
        {
            openAudio.Play();
            this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
        else
        {
            closeAudio.Play();
            this.gameObject.layer = LayerMask.NameToLayer("BlockingInteractable");
        }
    }
}
