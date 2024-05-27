using System;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : ScreenWrapObject
{

    [SerializeField] private SpaceshipParameters defaultShipParameters; 
    [SerializeField] private GameObject projectileSpawnPoint;
    [SerializeField] private GameObject projectile;

    protected Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private CircleCollider2D _collider2D;
    private Quaternion _startRotation;
    
    protected List<Projectile> MyProjectiles = new List<Projectile>();
    protected Vector3 MovementInput;
    
    private float _currentSpeed;
    private float _lastImpulseTimeStamp;
    private float _currentHealth;
    private float _velocityBeforeImpulse;
    
    private static readonly int Exp = Animator.StringToHash("expl");
    protected const string Projectile = "Projectile";
    
    protected SpaceshipParameters CurrentSpaceShipParameters { get; set; }
    public bool CanPlay { get; set; }

    protected GameObject ProjectileSpawnPoint => projectileSpawnPoint;
    protected bool CanImpulse => Time.unscaledTime > _lastImpulseTimeStamp + CurrentSpaceShipParameters.impulseCoolDown;
    
    private bool IsRotating => MovementInput != Vector3.zero;
   
    private float CurrentHealthPercentage => Mathf.Clamp01(_currentHealth / CurrentSpaceShipParameters.maxHealth);
    

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        _startRotation = transform.rotation;

    }

    public void SetUpSpaceship(Vector3 startPosition)
    {
        CanPlay = false;
        transform.position = startPosition;
        transform.rotation = _startRotation;
    }
    
    public virtual void StartGame()
    {
        SetShipParameters();
        _lastImpulseTimeStamp = Time.unscaledTime;
        _currentSpeed = CurrentSpaceShipParameters.speed;
        _currentHealth = CurrentSpaceShipParameters.maxHealth;
        CanPlay = true;
    }

    protected virtual void Update()
    {
        if(!CanPlay) return;
        if (CurrentHealthPercentage <= 0)
        {
            CanPlay = false;
            TerminateSpaceship();
        }
        MoveSpaceship();
     
        if(!IsOutsideScreen(_collider2D.radius/2)) return;
        WrapPosition();
    }

    protected virtual void FixedUpdate()
    {
        if(!CanPlay) return;
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

    public void FinishedGame()
    {
        CanPlay = false;
        gameObject.SetActive(false);
    }
    protected virtual void TerminateSpaceship()
    {
        CanPlay = false;
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
