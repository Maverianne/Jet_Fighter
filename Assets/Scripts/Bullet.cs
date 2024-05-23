using System.Collections;
using UnityEngine;

public class Bullet : ScreenWrapObject
{

    [SerializeField] private float bulletSpeed;
    [SerializeField] private float maxDistanceTraveled; 

    private Rigidbody _rigidbody;
    private SphereCollider _collider;
    private Vector2 _lastPosition;
    private float _totalDistance;

    private bool TerminateBullet => _totalDistance > maxDistanceTraveled;

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
        _totalDistance = 0;
        StartCoroutine(PerformTerminate());
    }

    private void Update()
    {
        if(!IsOutsideScreen(_collider.radius/2)) return;
        WrapPosition();
    }

    private IEnumerator PerformTerminate()
    {
        while (!TerminateBullet)
        {
            var position = transform.position;
            var distance = Vector3.Distance( _lastPosition, position ) ;
            _totalDistance += distance;
            _lastPosition = position;
            yield return null;
        }
        Terminate();
    }

    private void Terminate()
    {
        Destroy(gameObject);
    }
}
