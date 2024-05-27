using System;
using Managers;
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(fileName = "EnemyBehaviourParameters", menuName = "JetFighter/EnemyBehaviourParameters", order = 1)]
    public class EnemyBehaviourParameters : ScriptableObject
    {
        [SerializeField] private EnemyParameters[] enemyParameters;


        public EnemyParameters GetBehaviourParameters(GameplayManager.Difficulty difficulty)
        {
            foreach (var parameter in enemyParameters)
            {
                if(parameter.difficulty != difficulty) continue;
                return parameter;
            }

            return new EnemyParameters();
        }
        
        [Serializable]
        public class EnemyParameters
        {
          

            public GameplayManager.Difficulty difficulty = GameplayManager.Difficulty.Normal;
            public  GameplayParameters.SpaceshipParameters shipParameters;

            [Header("Enemy Parameters")] 
            
            [Header("Delays and cool downs")]
            [Range(0f, 5f)]
            public float minAttackCoolDown;
            [Range(0f, 5f)]
            public float maxAttackCoolDown;
            [Range(0f, 5f)]
            public float minStandbyTime;
            [Range(0f, 5f)]
            public float maxStandbyTime;
            [Range(0f, 5f)]
            public float projectileDetectionCoolDown;
            
            [Header("Distance behaviours")]
            [Range(0f, 5f)]
            public float attackDistance;
            [Range(1f, 5f)]
            public float retreatDistance;

            [Header("Probabilities")]
            [Range(0f, 1f)]
            public float impulseChance;
            [Range(0f, 1f)]
            public float dodgeBulletChance;
        }
    }
}