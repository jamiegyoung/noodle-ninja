using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Door : MonoBehaviour, Interactable
{
    private Animator anim;
    private Collider2D coll;
    private ShadowCaster2D shadowCaster;

    private void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        shadowCaster = GetComponent<ShadowCaster2D>();
    }

    public void Interact()
    {
        anim.SetBool("isOpen", !anim.GetBool("isOpen"));
        coll.isTrigger = !coll.isTrigger;
        shadowCaster.selfShadows = !shadowCaster.selfShadows;
        shadowCaster.castsShadows = !shadowCaster.castsShadows;
    }

}
