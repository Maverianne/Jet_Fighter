using System;
using UnityEngine;

namespace EnemyAI
{
    [CreateAssetMenu(fileName = "EnemyBehaviourParameters", menuName = "JetFighter/EnemyBehaviourParameters", order = 1)]
    public class EnemyBehaviourParameters : ScriptableObject
    {
        [SerializeField] private EnemyParameters[] enemyParameters;


        public EnemyParameters GetBehaviourParameters(EnemyParameters.Difficulty difficulty)
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
            public enum Difficulty 
            {
                None = 0, 
                Easy = 1, 
                Normal = 2, 
                Hard = 3
            }

            public Difficulty difficulty = Difficulty.Normal;
            public Spaceship.SpaceshipParameters shipParameters; 
            [Header("Enemy Parameters")]
            public float minAttackCoolDown;
            public float maxAttackCoolDown;
            public float attackDistance;
            public float retreatDistance;
        }
    }
}