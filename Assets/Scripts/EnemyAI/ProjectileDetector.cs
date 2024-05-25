using System;
using Unity.VisualScripting;
using UnityEngine;

namespace EnemyAI
{
    public class ProjectileDetector : MonoBehaviour
    {

        private EnemyShip _enemyShip;
        
        private const string Projectile = "Projectile";

        private void Awake()
        {
            _enemyShip = GetComponentInParent<EnemyShip>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag(Projectile)) return;
            _enemyShip.DodgeBullet = true;
        }
    }
}