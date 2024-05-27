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
    private Quaternion _startRotation;
    private Rigidbody2D _rigidbody2D;

    protected CircleCollider2D Collider2D;
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
    private float CurrentHealthPercentage => Mathf.Clamp01(_currentHealth / CurrentSpaceShipParameters.maxHealth);
    public string SpaceshipName => spaceshipName;
    public int MyScore => _myScore;
    private bool IsImpulsed => Time.unscaledTime < _lastImpulseTimeStamp + CurrentSpaceShipParameters.impulseDuration && _firstImpulseDone;
    private bool IsRotating => MovementInput != Vector3.zero;
    

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        Collider2D = GetComponent<CircleCollider2D>();
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
     
        if(!IsOutsideScreen(Collider2D.radius/2)) return;
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
        _firstImpulseDone = false;
        
        //register players and get past info
        _myScore = MainManager.Instance.GameplayManager.GetScore(spaceshipName);
        MainManager.Instance.GameplayManager.AttemptAddData(spaceshipName, _myScore);
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
        var step = (Time.deltaTime * GetRotatingSpeed() * 100);
        transform.Rotate(movementInput * step);
    }
    
    protected virtual void Impulse()
    {
        if(!MyStats.CanImpulse) return;
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
    
    protected virtual void TerminateSpaceship()
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
 
    public void DisableShip()
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
