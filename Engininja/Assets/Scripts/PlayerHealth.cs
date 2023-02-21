using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    public GameObject deathCanvas;
    public bool isDead;
    public Volume globalVolume;
    private Vignette vignette;
    private FilmGrain filmGrain;
    private LensDistortion lensDistortion;
    public int health = 3;
    public float transitionSpeed;
    private float timeSinceLastDamage = 0f;

    private void FixedUpdate()
    {
        // Health Regeneration
        if (timeSinceLastDamage - Time.time < -10f && health < 3)
        {
            timeSinceLastDamage = Time.time;
            health += 1;
        }

        float redAmount = 0f;
        // No case 0 as time stops
        switch (health)
        {
            case 1:
                redAmount = Mathf.Lerp(vignette.color.value.r, 240f / 255f, transitionSpeed);
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, .50f, transitionSpeed);
                filmGrain.intensity.value = Mathf.Lerp(filmGrain.intensity.value, .7f, transitionSpeed);
                lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, .3f, transitionSpeed);

                break;
            case 2:
                redAmount = Mathf.Lerp(vignette.color.value.r, 140f / 255f, transitionSpeed);
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, .45f, transitionSpeed);
                filmGrain.intensity.value = Mathf.Lerp(filmGrain.intensity.value, .4f, transitionSpeed);
                lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, .25f, transitionSpeed);
                break;
            case 3:
                redAmount = Mathf.Lerp(vignette.color.value.r, 0, transitionSpeed);
                vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, .35f, transitionSpeed);
                filmGrain.intensity.value = Mathf.Lerp(filmGrain.intensity.value, .3f, transitionSpeed);
                lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, .2f, transitionSpeed);
                break;
        }

        vignette.color.value = new Color(redAmount, 0, 0);
    }

    private void Start()
    {
        globalVolume.profile.TryGet<Vignette>(out vignette);
        globalVolume.profile.TryGet<FilmGrain>(out filmGrain);
        globalVolume.profile.TryGet<LensDistortion>(out lensDistortion);
    }

    void Update()
    {
        if (health <= 0 && !isDead)
        {
            deathCanvas.SetActive(true);
        }

        // Handle case 0 here:
        if (health == 0)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, .5f, transitionSpeed);
            filmGrain.intensity.value = Mathf.Lerp(filmGrain.intensity.value, .3f, transitionSpeed);
            lensDistortion.intensity.value = Mathf.Lerp(lensDistortion.intensity.value, .2f, transitionSpeed);
        }
    }

    public void Damage()
    {
        if (health > 0)
        {
            timeSinceLastDamage = Time.time;
            health -= 1;
        }
    }

    public override string ToString() => String.Format("{0}HP", health.ToString());
}
