using UnityEngine;
using UnityEngine.InputSystem;

public class Plane : ScreenWrapObject
{

    [SerializeField] private float planeSpeed;
    [SerializeField] private float planeSpeedIncrement;
    [SerializeField] private float turningPlaneSpeed;
    [SerializeField] private float impulseSpeed;
    [SerializeField] private float impulseCoolDown; 
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float maxHealth;

    private Rigidbody2D _rigidbody2D;
    private CircleCollider2D _collider2D;

    private bool _isRotating;
    private float _currentSpeed;
    private float _lastImpulseTimeStamp;

    private float _velocityBeforeImpulse;

    private bool CanImpulse => Time.unscaledTime > _lastImpulseTimeStamp + impulseCoolDown;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<CircleCollider2D>();
        _currentSpeed = planeSpeed;
    }

    private void Update()
    {
        MoveSpaceShip();
        if (Keyboard.current.dKey.isPressed) Rotate(true);
        if (Keyboard.current.aKey.isPressed) Rotate(false);
        if (Keyboard.current.wKey.wasPressedThisFrame) Impulse();
        if (Keyboard.current.sKey.wasPressedThisFrame) Shooting();
        if (Keyboard.current.dKey.wasReleasedThisFrame || Keyboard.current.aKey.wasReleasedThisFrame) _isRotating = false;
        if(!IsOutsideScreen(_collider2D.radius/2)) return;
        WrapPosition();
    }
    

    private void MoveSpaceShip()
    {
        transform.position -= transform.up * (Time.deltaTime * GetSpeed());
    }
    private void Rotate(bool rotatingRight)
    {
        _isRotating = true;
        var rotatingVector = rotatingRight ? Vector3.back : Vector3.forward;
        transform.Rotate(rotatingVector * (Time.deltaTime * rotatingSpeed * 100));
    }
    
    private float GetSpeed()
    {
        if (_isRotating)
        {
            _currentSpeed = turningPlaneSpeed;
            return _currentSpeed;
        }

        if (_currentSpeed > planeSpeed) return _currentSpeed;
        
        _currentSpeed += planeSpeedIncrement;
        return _currentSpeed;
    }

    private void Impulse()
    {
         if(!CanImpulse) return;
        _lastImpulseTimeStamp = Time.unscaledTime;
        _rigidbody2D.AddForce(-transform.up * impulseSpeed);
    }

    private void Shooting()
    { 
        var bulletPrefab = Instantiate(bullet);
        bulletPrefab.GetComponent<Bullet>().InitializeBullet(transform);
    }
}
