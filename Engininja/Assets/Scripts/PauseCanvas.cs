using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseCanvas : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public PauseController pauseController;

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

    public void Resume()
    {
        pauseController.TogglePause();
    }
}
