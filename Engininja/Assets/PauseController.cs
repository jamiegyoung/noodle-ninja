using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{

    public PlayerInput playerInput;
    public GameObject pauseUI;
    private InputHandler inputHandler;
    private bool paused;
    // Update is called once per frame


    private void Start()
    {
        inputHandler = new InputHandler(playerInput);
    }
    void Update()
    {
        if (inputHandler.WasPressedThisFrame(InputHandlerActions.Pause))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (paused)
        {
            paused = false;
            Time.timeScale = 1.0f;
            pauseUI.SetActive(false);
        }
        else
        {
            paused = true;
            Time.timeScale = 0f;
            pauseUI.SetActive(true);
        }
    }
}
