using System.Collections;
using UnityEngine;

public class Projectile : ScreenWrapObject
{
    
    private float _maxDistanceTraveled;
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _collider;
    private Vector2 _lastPosition;
    private float _totalDistance;

    private bool TerminateBullet => _totalDistance > _maxDistanceTraveled;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
    }

    public void InitializeBullet( Vector3 spawnPos, Quaternion spawnRotation, float speed, float distance)
    {
        _collider.enabled = false;
        transform.position = spawnPos;
        transform.rotation = spawnRotation;
        _maxDistanceTraveled = distance;
        _rigidbody.AddForce(-transform.up * (speed * 100));
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
        yield return new WaitForSeconds(0.1f);
        _collider.enabled = true;
        while (!TerminateBullet)
        {
            var position = transform.position;
            var distance = Vector3.Distance( _lastPosition, position ) ;
            _totalDistance += distance;
            _lastPosition = position;
            yield return null;
        }
        TerminateProjectile();
    }

    private void TerminateProjectile()
    {
        Destroy(gameObject);
    }
}
