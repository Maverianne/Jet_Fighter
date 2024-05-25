using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Spaceship : ScreenWrapObject
{

    [SerializeField] private SpaceshipParameters defaultShipParameters; 
    [SerializeField] private GameObject projectileSpawnPoint;
    [SerializeField] private GameObject projectile;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    protected CircleCollider2D Collider2D;

    protected Vector3 MovementInput;
    
    protected bool IsDestroyed; 
    private float _currentSpeed;
    private float _lastImpulseTimeStamp;
    private float _currentHealth;
    private float _velocityBeforeImpulse;
    private SpaceshipParameters _currentSpaceShipParameter;

    protected bool CanImpulse => Time.unscaledTime > _lastImpulseTimeStamp + CurrentSpaceShipParameters.impulseCoolDown;
    private bool IsRotating => MovementInput != Vector3.zero;
    protected float CurrentHealthPercentage => Mathf.Clamp01(_currentHealth / CurrentSpaceShipParameters.maxHealth);


    protected SpaceshipParameters CurrentSpaceShipParameters
    {
        get => defaultShipParameters;
        set => _currentSpaceShipParameter = value;
    }

    private static readonly int Exp = Animator.StringToHash("expl");
    private const string Projectile = "Projectile";

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        Collider2D = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        
    }

    private void Start()
    {
        StartGame();
    }

    protected virtual void StartGame()
    {
        _currentSpeed = CurrentSpaceShipParameters.speed;
        _currentHealth = CurrentSpaceShipParameters.maxHealth;
        IsDestroyed = false;
    }

    private void Update()
    {
        if(IsDestroyed)return;
        if (CurrentHealthPercentage <= 0)
        {
            IsDestroyed = true;
            TerminateSpaceship();
        }
        MoveSpaceship();
     
        if(!IsOutsideScreen(Collider2D.radius/2)) return;
        WrapPosition();
    }

    private void FixedUpdate()
    {
        Rotate(MovementInput);
    }

    protected void MoveSpaceship()
    {
        transform.position -= transform.up * (Time.deltaTime * GetSpeed());
    }

    protected virtual void Rotate(Vector3 movementInput)
    {
        transform.Rotate(movementInput * (Time.deltaTime * CurrentSpaceShipParameters.rotatingSpeed * 100));
    }
    
    private float GetSpeed()
    {
        if (IsRotating)
        {
            _currentSpeed = CurrentSpaceShipParameters.speedWhileRotating;
            return _currentSpeed;
        }

        if (_currentSpeed > CurrentSpaceShipParameters.speed) return _currentSpeed;
        
        _currentSpeed += CurrentSpaceShipParameters.speedIncrement;
        return _currentSpeed;
    }

    protected void Impulse()
    {
         if(!CanImpulse) return;
        _lastImpulseTimeStamp = Time.unscaledTime;
        _rigidbody2D.AddForce(-transform.up * CurrentSpaceShipParameters.impulseSpeed);
    }

    protected void Shooting()
    { 
        var bulletPrefab = Instantiate(projectile);
        bulletPrefab.GetComponent<Projectile>().InitializeBullet(projectileSpawnPoint.transform.position, transform.rotation);
    }

    protected void TerminateSpaceship()
    {
        _animator.SetBool(Exp, true);
    }

    private void DestroyObject()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag(Projectile)) return;
        _currentHealth--;
    }

    [Serializable]
    public class SpaceshipParameters
    {
        public float maxHealth;
        [Header("Speed parameters")]
        public float speed;
        public float speedIncrement;
        public float speedWhileRotating;
        public float rotatingSpeed;
        [Header("Impulse parameters")]
        public float impulseSpeed;
        public float impulseCoolDown;
    }
}
