using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayParameters", menuName = "JetFighter/GameplayParameters", order = 1)]
public class GameplayParameters : ScriptableObject
{
    [SerializeField] private SpaceshipParameters playableSpaceshipParameters;

    public SpaceshipParameters PlayableSpaceshipParameters => playableSpaceshipParameters;
    
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
        public float impulseDuration;
        public float impulseCoolDown;
        [Header("Projectile parameters")]
        public float projectileSpeed; 
        public float projectileDistance;
    }
}