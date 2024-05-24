using UnityEngine;

public class Spaceship : ScreenWrapObject
{

    [SerializeField] private float maxHealth;
    [SerializeField] private float speed;
    [SerializeField] private float speedIncrement;
    [SerializeField] private float speedWhileRotating;
    [SerializeField] private float rotatingSpeed;
    [SerializeField] private float impulseSpeed;
    [SerializeField] private float impulseCoolDown; 
    [SerializeField] private GameObject projectileSpawnPoint;
    [SerializeField] private GameObject projectile;

    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private CircleCollider2D _collider2D;

    protected Vector3 MovementInput;
    
    private bool _isDestroyed; 
    private float _currentSpeed;
    private float _lastImpulseTimeStamp;
    private float _currentHealth;
    private float _velocityBeforeImpulse;

    private bool CanImpulse => Time.unscaledTime > _lastImpulseTimeStamp + impulseCoolDown;
    private bool IsRotating => MovementInput != Vector3.zero;
    private float CurrentHealthPercentage => Mathf.Clamp01(_currentHealth / maxHealth);

    private static readonly int Exp = Animator.StringToHash("expl");
    private const string Projectile = "Projectile";

    protected override void Awake()
    {
        base.Awake();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        _currentSpeed = speed;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _currentHealth = maxHealth;
        _isDestroyed = false;
    }

    private void Update()
    {
        if(_isDestroyed)return;
        if (CurrentHealthPercentage <= 0)
        {
            _isDestroyed = true;
            TerminateSpaceship();
        }
        MoveSpaceship();
        Rotate(MovementInput);
        if(!IsOutsideScreen(_collider2D.radius/2)) return;
        WrapPosition();
    }
    

    private void MoveSpaceship()
    {
        transform.position -= transform.up * (Time.deltaTime * GetSpeed());
    }

    private void Rotate(Vector3 movementInput)
    {
        transform.Rotate(movementInput * (Time.deltaTime * rotatingSpeed * 100));
    }
    
    private float GetSpeed()
    {
        if (IsRotating)
        {
            _currentSpeed = speedWhileRotating;
            return _currentSpeed;
        }

        if (_currentSpeed > speed) return _currentSpeed;
        
        _currentSpeed += speedIncrement;
        return _currentSpeed;
    }

    protected void Impulse()
    {
         if(!CanImpulse) return;
        _lastImpulseTimeStamp = Time.unscaledTime;
        _rigidbody2D.AddForce(-transform.up * impulseSpeed);
    }

    protected void Shooting()
    { 
        var bulletPrefab = Instantiate(projectile);
        bulletPrefab.GetComponent<Projectile>().InitializeBullet(projectileSpawnPoint.transform.position, transform.rotation);
    }

    private void TerminateSpaceship()
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
}
