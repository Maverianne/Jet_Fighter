using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Spaceship
{
    public override void StartGame()
    {
        base.StartGame();
        MyStats.SetInfo(MyScore, SpaceshipName, CurrentSpaceShipParameters.impulseCoolDown);
    }

    protected override void TerminateSpaceship()
    {
        base.TerminateSpaceship();
        MainManager.Instance.GameplayManager.GameRoundDone(false);
    }

    protected override float GetRotatingSpeed()
    {
        //Stop rotation so player doesn't accidentally rotate out of screen
        var speed = 0f;
        if (IsOutsideScreen(0)) return speed;
        return CanCheckScreenBounds ? CurrentSpaceShipParameters.rotatingSpeed  : speed;
    }

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
}