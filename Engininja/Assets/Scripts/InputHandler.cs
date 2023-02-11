using System;
using UnityEngine.InputSystem;

public enum InputHandlerActions
{
    Move,
    Attack
}

public class InputHandler
{
    readonly PlayerInput playerInput;

    public InputHandler(PlayerInput input)
    {
        playerInput = input;
    }

    private InputAction GetAction(InputHandlerActions inputType)
    {
        return playerInput.actions[Enum.GetName(typeof(InputHandlerActions), inputType)];
    }

    public T GetActionValue<T>(InputHandlerActions inputType) where T : struct
    {
        return GetAction(inputType).ReadValue<T>();
    }

    public bool WasPressedThisFrame(InputHandlerActions inputType)
    {
        return GetAction(inputType).WasPressedThisFrame();
    }

    public string GetBindingDisplayString(InputHandlerActions inputType)
    {
        return GetAction(inputType).GetBindingDisplayString();
    }
}