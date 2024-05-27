using Managers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Spaceship
{
    public ImpulseSlider MySlider { get; set; }

    public override void StartGame()
    {
        base.StartGame();
        MySlider.ResetSlider(CurrentSpaceShipParameters.impulseCoolDown);
    }

    protected override void Impulse()
    {
        base.Impulse();
        MySlider.ResetSlider(CurrentSpaceShipParameters.impulseCoolDown);
    }

    protected override void TerminateSpaceship()
    {
        base.TerminateSpaceship();
        MainManager.Instance.GameplayManager.GameDone(false);
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