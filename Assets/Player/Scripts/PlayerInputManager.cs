using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ContiniousActions
{
    public Vector2 MoveInput;
    public Vector2 PrevMoveInput;
}
public class PlayerInputManager : MonoBehaviour
{
    public UnityEvent<Vector2EventArgs> OnMoveEvent;
    public UnityEvent<PressedStateEventArgs> OnRunEvent;
    public UnityEvent<PressedStateEventArgs> OnCrouchEvent;
    public UnityEvent<PressedStateEventArgs> OnJumpEvent;
    public UnityEvent<PressedStateEventArgs> OnInteractEvent;
    public UnityEvent<PressedStateEventArgs> OnPauseEvent;
    public UnityEvent<PressedStateEventArgs> OnShootEvent;
    public ContiniousActions ContiniousActions { get; private set; } = new();

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();
        ContiniousActions.MoveInput = value;
        OnMoveEvent?.Invoke(new Vector2EventArgs(value));
    }

    public void OnRun(InputAction.CallbackContext ctx)
    {
        PressedState state = GetPressedState(ctx);
        OnRunEvent?.Invoke(new PressedStateEventArgs(state));
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        PressedState state = GetPressedState(ctx);
        OnCrouchEvent?.Invoke(new PressedStateEventArgs(state));
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        PressedState state = GetPressedState(ctx);
        OnJumpEvent?.Invoke(new PressedStateEventArgs(state));
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        PressedState state = GetPressedState(ctx);
        OnInteractEvent?.Invoke(new PressedStateEventArgs(state));
    }

    public void OnPause(InputAction.CallbackContext ctx)
    {
        PressedState state = GetPressedState(ctx);
        OnPauseEvent?.Invoke(new PressedStateEventArgs(state));
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        PressedState state = GetPressedState(ctx);
        OnShootEvent?.Invoke(new PressedStateEventArgs(state));
    }

    private PressedState GetPressedState(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled) return PressedState.Canceled;
        if (ctx.started) return PressedState.Started;
        return PressedState.Performed;
    }

    public enum PressedState
    {
        Started = 1,
        Performed = 2,
        Canceled = 0
    }

    public class PressedStateEventArgs : EventArgs
    {
        public PressedState State { get; private set; }
        public PressedStateEventArgs(PressedState state) { State = state; }
    }

    public class Vector2EventArgs : EventArgs
    {
        public Vector2 Value { get; private set; }
        public Vector2EventArgs(Vector2 value) { Value = value; }
    }

    public class TriggerEventArgs : EventArgs
    {
        public float Value { get; private set; }
        public TriggerEventArgs(float value) { Value = value; }
    }
}