using UnityEngine;

public class Bullet : ScreenWrapObject
{

    [SerializeField] private float bulletSpeed;

    private Rigidbody _rigidbody;
    private SphereCollider _collider;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
    }

    public void InitializeBullet( Transform spawnTransform)
    {
        transform.position = spawnTransform.position;
        transform.rotation = spawnTransform.rotation;
        _rigidbody.AddForce(-transform.up * (bulletSpeed * 100));
        
    }

    private void Update()
    {
        if(!IsOutsideScreen(_collider.radius/2)) return;
        WrapPosition();
    }
}
