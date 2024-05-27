using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;

public class Spaceship : ScreenWrapObject
{
    [SerializeField] private string spaceshipName;
    [SerializeField] private GameObject projectileSpawnPoint;
    [SerializeField] private GameObject projectile;

    private Animator _animator;
    private CircleCollider2D _collider2D;
    private Quaternion _startRotation;
    
    protected Rigidbody2D Rigidbody2D;
    protected List<Projectile> MyProjectiles = new List<Projectile>();
    protected Vector3 MovementInput;

    private int _myScore;
    private float _currentSpeed;
    private float _lastImpulseTimeStamp;
    private float _currentHealth;
    private float _velocityBeforeImpulse;
    
    public PlayerInfo MyStats { get; set; }
    private static readonly int Exp = Animator.StringToHash("expl");
    protected const string Projectile = "Projectile";
    
    protected GameplayParameters.SpaceshipParameters CurrentSpaceShipParameters { get; set; }
    public bool CanPlay { get; set; }

    protected GameObject ProjectileSpawnPoint => projectileSpawnPoint;
    private bool CanImpulse => Time.unscaledTime > _lastImpulseTimeStamp + CurrentSpaceShipParameters.impulseCoolDown;

    private bool IsImpulsed => Time.unscaledTime < _lastImpulseTimeStamp + CurrentSpaceShipParameters.impulseDuration;
    
    private bool IsRotating => MovementInput != Vector3.zero;
    private float CurrentHealthPercentage => Mathf.Clamp01(_currentHealth / CurrentSpaceShipParameters.maxHealth);
    

    protected override void Awake()
    {
        base.Awake();
        Rigidbody2D = GetComponent<Rigidbody2D>();
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
        _myScore = MainManager.Instance.GameplayManager.GetScore(spaceshipName);
        MyStats.SetInfo(_myScore, spaceshipName, CurrentSpaceShipParameters.impulseCoolDown);
    } 
    protected virtual void Update()
    {
        if(!CanPlay) return;
        if (CurrentHealthPercentage <= 0)
        {
            CanPlay = false;
            TerminateSpaceship();
        }
     
        if(!IsOutsideScreen(_collider2D.radius/2)) return;
        WrapPosition();
    }

    protected virtual void FixedUpdate()
    {
        if(!CanPlay) return;
        Rotate(MovementInput);
        MoveSpaceship();
    }

    protected void MoveSpaceship()
    {
        if(IsImpulsed) return;
        Rigidbody2D.velocity = -transform.up  * (Time.fixedDeltaTime * GetSpeed() * 100);
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
         MyStats.ResetImpulseSlider(CurrentSpaceShipParameters.impulseCoolDown);
        _lastImpulseTimeStamp = Time.unscaledTime;
        Rigidbody2D.AddForce(-transform.up * (CurrentSpaceShipParameters.impulseSpeed), ForceMode2D.Impulse);
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
        CurrentSpaceShipParameters = MainManager.Instance.GameplayParameters.PlayableSpaceshipParameters;
    }

    private void Damaged()
    {
        _currentHealth--;
        MyStats.SetHealthBar(CurrentHealthPercentage);
    }

    public void FinishedGame()
    {
        CanPlay = false;
        _myScore++;
        MainManager.Instance.GameplayManager.AttemptAddData(spaceshipName, _myScore);
        MyStats.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    private void TerminateSpaceship()
    {
        CanPlay = false;
        MyStats.gameObject.SetActive(false);
        _animator.SetBool(Exp, true);
    }

    protected virtual void DestroyObject()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.CompareTag(Projectile)) return;
        col.gameObject.GetComponent<Projectile>().TerminateProjectile();
        Damaged();
    }
}
