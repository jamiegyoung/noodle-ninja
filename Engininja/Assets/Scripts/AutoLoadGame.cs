using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLoadGame : MonoBehaviour
{
    public SceneLoader sceneLoader;

    // Start is called before the first frame update
    void OnEnable()
    {
        sceneLoader.LoadLevel(SceneLoader.Levels.Game);
    }
}
