using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    private Animator anim;
    private Collider2D coll;

    private void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    public void Interact()
    {
        anim.SetBool("isOpen", !anim.GetBool("isOpen"));
        coll.isTrigger = !coll.isTrigger;
    }

}
