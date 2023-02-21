using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCanvas : MonoBehaviour
{
    public SceneLoader sceneLoader;

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void Retry()
    {
        Debug.Log("Restarting");
        Time.timeScale = 0;
        sceneLoader.LoadLevel(SceneLoader.Levels.Game);
    }

    public void MainMenu()
    {
        sceneLoader.LoadLevel(SceneLoader.Levels.Menu);
    }
}
