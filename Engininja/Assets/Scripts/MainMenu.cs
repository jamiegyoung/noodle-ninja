using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    
    public void Play()
    {
        sceneLoader.LoadLevel(SceneLoader.Levels.Intro);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
