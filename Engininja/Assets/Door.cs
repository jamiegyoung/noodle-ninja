using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, Interactable
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact()
    {
        anim.SetBool("isOpen", !anim.GetBool("isOpen"));
    }

}
