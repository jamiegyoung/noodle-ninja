using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Switch : MonoBehaviour, Interactable
{
    private Animator anim;
    private bool isOn = true;
    private Light2D light2d;
    public bool IsInteractable => true;
    public Doors doors;
    public GameObject ceilingLights;
    private Light2D[] _ceilingLights;

    public void Interact()
    {
        isOn = !isOn;
        doors.setLocked(isOn);
        light2d.color = isOn ? Color.green : Color.red;
        if (isOn == true)
        {
            doors.setClosed();
        }
        anim.SetBool("switchIsOn", isOn);

        foreach (Light2D light in _ceilingLights)
        {
            light.intensity = isOn ? 0.9f : 0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        light2d = GetComponent<Light2D>();
        _ceilingLights = ceilingLights.GetComponentsInChildren<Light2D>();

    }
}
