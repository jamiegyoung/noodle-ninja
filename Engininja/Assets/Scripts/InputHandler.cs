using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public enum InputHandlerActions
{
    Move,
}

public class InputHandler
{
    PlayerInput playerInput;
    public InputHandler(PlayerInput input)
    {
        playerInput = input;
    }

    public T getActionValue<T>(InputHandlerActions inputType) where T : struct
    {
        return playerInput.actions[Enum.GetName(typeof(InputHandlerActions), inputType)].ReadValue<T>();
    }
}