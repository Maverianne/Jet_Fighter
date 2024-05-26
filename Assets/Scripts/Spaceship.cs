using System;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : ScreenWrapObject
{

    [SerializeField] private SpaceshipParameters defaultShipParameters; 
    [SerializeField] private GameObject projectileSpawnPoint;
    [SerializeField] private GameObject projectile;


    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private CircleCollider2D _collider2D;

    protected Vector3 MovementInput;
    protected List<Projectile> MyProjectiles = new List<Projectile>();

    private bool _isDestroyed; 
    private float _currentSpeed;
    private float _lastImpulseTimeStamp;
    private float _currentHealth;
    private float _velocityBeforeImpulse;

    private bool CanImpulse => Time.unscaledTime > _lastImpulseTimeStamp + CurrentSpaceShipParameters.impulseCoolDown;
    private bool IsRotating => MovementInput != Vector3.zero;
    private float CurrentHealthPercentage => Mathf.Clamp01(_currentHealth / CurrentSpaceShipParameters.maxHealth);


    protected SpaceshipParameters CurrentSpaceShipParameters { get; set; }
    protected GameObject ProjectileSpawnPoint => projectileSpawnPoint;
    

    private static readonly int Exp = Animator.StringToHash("expl");
    protected const string Projectile = "Projectile";

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        
    }

    private void Start()
    {
        StartGame();
    }

    protected virtual void StartGame()
    {
        SetShipParameters();
        _currentSpeed = CurrentSpaceShipParameters.speed;
        _currentHealth = CurrentSpaceShipParameters.maxHealth;
        _isDestroyed = false;
    }

    protected virtual void Update()
    {
        if(_isDestroyed)return;
        if (CurrentHealthPercentage <= 0)
        {
            _isDestroyed = true;
            TerminateSpaceship();
        }
        MoveSpaceship();
     
        if(!IsOutsideScreen(_collider2D.radius/2)) return;
        WrapPosition();
    }

    protected virtual void FixedUpdate()
    {
        Rotate(MovementInput);
    }

    private void MoveSpaceship()
    {
        transform.position -= transform.up * (Time.deltaTime * GetSpeed());
    }

    private void Rotate(Vector3 movementInput)
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

    protected virtual void Impulse()
    {
         if(!CanImpulse) return;
        _lastImpulseTimeStamp = Time.unscaledTime;
        _rigidbody2D.AddForce(-transform.up * CurrentSpaceShipParameters.impulseSpeed);
    }

    protected virtual void Shooting()
    { 
        var projectilePrefabGo = Instantiate(projectile);
        var projectilePrefab = projectilePrefabGo.GetComponent<Projectile>();
        projectilePrefab.InitializeBullet(projectileSpawnPoint.transform.position, transform.rotation, CurrentSpaceShipParameters.projectileSpeed, CurrentSpaceShipParameters.projectileDistance);
        MyProjectiles.Add(projectilePrefab);
    }

    protected virtual void SetShipParameters()
    {
        CurrentSpaceShipParameters = defaultShipParameters;
    }

    protected virtual void TerminateSpaceship()
    {
        _animator.SetBool(Exp, true);
    }

    private void DestroyObject()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.CompareTag(Projectile)) return;
        col.gameObject.GetComponent<Projectile>().TerminateProjectile();
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
        [Header("Projectile parameters")]
        public float projectileSpeed; 
        public float projectileDistance;
    }
}
