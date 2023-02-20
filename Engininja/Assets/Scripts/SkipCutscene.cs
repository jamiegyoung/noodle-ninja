using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class SkipCutscene : MonoBehaviour
{
    public SceneLoader sceneLoader;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Skipping");
            sceneLoader.LoadLevel(SceneLoader.Levels.Game);
        }
    }
}
