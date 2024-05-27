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

    private Rigidbody2D _rigidbody2D;
    protected List<Projectile> MyProjectiles = new List<Projectile>();
    protected Vector3 MovementInput;

    private int _myScore;
    private bool _firstImpulseDone;
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

    private bool IsImpulsed => Time.unscaledTime < _lastImpulseTimeStamp + CurrentSpaceShipParameters.impulseDuration && _firstImpulseDone;
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


    #region SetUp / StartUp

    public void SetUpSpaceship(Vector3 startPosition)
    {
        //Placing Ship on position
        CanPlay = false;
        transform.position = startPosition;
        transform.rotation = _startRotation;
    }
    
    public virtual void StartGame()
    {
        //Actually startGame
        SetShipParameters();
        CanPlay = true;
        _lastImpulseTimeStamp = Time.unscaledTime;
        _currentSpeed = CurrentSpaceShipParameters.speed;
        _currentHealth = CurrentSpaceShipParameters.maxHealth;
        _myScore = MainManager.Instance.GameplayManager.GetScore(spaceshipName);
        _firstImpulseDone = false;
        MyStats.SetInfo(_myScore, spaceshipName, CurrentSpaceShipParameters.impulseCoolDown);
    } 
    
    protected virtual void SetShipParameters()
    {
        CurrentSpaceShipParameters = MainManager.Instance.GameplayParameters.PlayableSpaceshipParameters;
    }

    #endregion

    #region Movement / Attack

    protected void MoveSpaceship()
    {
        if(IsImpulsed) return;
        _rigidbody2D.velocity = -transform.up  * (Time.fixedDeltaTime * GetSpeed() * 100);
    }

    private void Rotate(Vector3 movementInput)
    {
        transform.Rotate(movementInput * (Time.deltaTime * GetRotatingSpeed() * 100));
    }
    
    protected virtual void Impulse()
    {
        if(!CanImpulse) return;
        _firstImpulseDone = true;
        MyStats.ResetImpulseSlider(CurrentSpaceShipParameters.impulseCoolDown);
        _lastImpulseTimeStamp = Time.unscaledTime;
        _currentSpeed = 0;
        _rigidbody2D.AddForce(-transform.up * CurrentSpaceShipParameters.impulseSpeed, ForceMode2D.Impulse);
    }
    
    protected virtual void Shooting()
    { 
        var projectilePrefabGo = Instantiate(projectile);
        var projectilePrefab = projectilePrefabGo.GetComponent<Projectile>();
        projectilePrefab.InitializeBullet(projectileSpawnPoint.transform.position, transform.rotation, CurrentSpaceShipParameters.projectileSpeed, CurrentSpaceShipParameters.projectileDistance);
        MyProjectiles.Add(projectilePrefab);
    }
    #endregion
    
    #region Damage / Terminate Spaceship

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(!col.gameObject.CompareTag(Projectile)) return;
        col.gameObject.GetComponent<Projectile>().TerminateProjectile();
        Damaged();
    }
    
    private void Damaged()
    {
        _currentHealth--;
        MyStats.SetHealthBar(CurrentHealthPercentage);
    }
    
    private void TerminateSpaceship()
    {
        //ship destroyed, play animation
        CanPlay = false;
        MyStats.gameObject.SetActive(false);
        _animator.SetBool(Exp, true);
    }

    public void FinishedGame()
    {
        //if still alive after game is done
        //this ship is the winner, add to score
        //disable ship
        
        CanPlay = false;
        _myScore++;
        MainManager.Instance.GameplayManager.AttemptAddData(spaceshipName, _myScore);
        MyStats.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
 
    protected virtual void DisableShip()
    {
        //disable on animation event
        gameObject.SetActive(false);
    }

    #endregion
    
    #region Getters

    protected virtual float GetRotatingSpeed()
    {
        return CurrentSpaceShipParameters.rotatingSpeed;
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

    #endregion
}
