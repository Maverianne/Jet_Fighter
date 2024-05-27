using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Spaceship
{
    private const float SmallImpulse = 10f;
    
    protected override void DestroyObject()
    {
        base.DestroyObject();
        MainManager.Instance.GameplayManager.GameDone(false);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector3>();
    }

    protected override void AddScreenOffImpulse()
    {
        Rigidbody2D.AddForce(-transform.up * SmallImpulse);
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
}