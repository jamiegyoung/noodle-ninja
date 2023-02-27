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
    public List<Light2D> enemyLights = new();
    private Light2D[] _ceilingLights;
    private AudioSource audioSource;
    public EnemyGenerator enemyGenerator;

    private IEnumerator ToggleLights()
    {
        foreach (Light2D light in _ceilingLights)
        {
            light.intensity = isOn ? 0.9f : 0f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void Interact(GameObject interactor)
    {
        isOn = !isOn;
        doors.setLocked(isOn);
        light2d.color = isOn ? Color.green : Color.red;
        if (isOn == true)
        {
            doors.setClosed();
        }
        anim.SetBool("switchIsOn", isOn);

        StartCoroutine(ToggleLights());
        StartCoroutine(enemyGenerator.SwitchEnemyLights());
        audioSource.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        light2d = GetComponent<Light2D>();
        audioSource = GetComponent<AudioSource>();
        _ceilingLights = ceilingLights.GetComponentsInChildren<Light2D>();
    }
}
