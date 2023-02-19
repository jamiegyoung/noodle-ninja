using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    
    public void Play()
    {
        sceneLoader.LoadLevel(SceneLoader.Levels.Game);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
