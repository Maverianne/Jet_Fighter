using Managers;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Spaceship
{

    public ImpulseSlider MySlider { get; set; }

    private const float SmallImpulse = 10f;
    public override void StartGame()
    {
        base.StartGame();
        MySlider.ResetSlider(CurrentSpaceShipParameters.impulseCoolDown);
    }

    protected override void Impulse()
    {
        if(!CanImpulse) return;
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

    protected override void AddScreenOffImpulse()
    {
        _rigidbody2D.AddForce(-transform.up * SmallImpulse);
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