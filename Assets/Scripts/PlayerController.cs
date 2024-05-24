using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Spaceship
{
    
    public void OnRotate(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector3>();
    }

    public void OnImpulse(InputAction.CallbackContext context)
    {
        if (!context.action.triggered) return;
        Impulse();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (!context.action.triggered) return;
        Shooting();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        
    }
}