using System;
using System.Collections;
using UnityEngine;

namespace EnemyAI
{
        public class EnemyShip : Spaceship
        {
                [SerializeField] private EnemyBehaviourParameters enemyBehaviourParameters;
                
                private IEnumerator _currentBehaviour;
                private PlayerController _player;
                private Transform _lastPlayerPos;

                private EnemyBehaviourParameters.EnemyParameters _currentEnemyParameters;
                private bool LookingAtPlayer => Math.Abs(transform.rotation.z - _lastPlayerPos.rotation.z) < _currentEnemyParameters.retreatDistance;
                private bool PlayerInAttackRange => Vector2.Distance(_player.transform.position, transform.position) < _currentEnemyParameters.attackDistance;
                private bool PlayerTooClose => Math.Abs(transform.rotation.z - _lastPlayerPos.rotation.z) < 0.1;

                protected override void Awake()
                {
                        base.Awake();
                        _player = FindObjectOfType<PlayerController>();
                }

                protected override void StartGame()
                {
                        _currentEnemyParameters = enemyBehaviourParameters.GetBehaviourParameters(EnemyBehaviourParameters.EnemyParameters.Difficulty.Normal);
                        CurrentSpaceShipParameters = _currentEnemyParameters.shipParameters;
                        base.StartGame();
                        SelectNextBehaviour();
                }

                protected override void Rotate(Vector3 movementInput)
                {
                        var rotation = transform.rotation;
                        
                        var difference =  _player.transform.position - transform.position;
                        var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                        var vectorTargetRotation = new Vector3(rotation.x, rotation.y, rotationZ);
                        transform.Rotate(vectorTargetRotation * (Time.deltaTime * _currentEnemyParameters.shipParameters.rotatingSpeed));

                }


                private void SelectNextBehaviour()
                {
                        StopCoroutine(_currentBehaviour);
                        if (PlayerInAttackRange)
                        {
                                _currentBehaviour = PerformApproachPlayer();
                        }
                        else if(PlayerTooClose)
                        { 
                                _currentBehaviour = PerformRetreat();
                        }
                        else
                        {
                                
                        }
                        StartCoroutine(_currentBehaviour);
                }

                private IEnumerator LookAtPlayer()
                {
                        _lastPlayerPos = _player.transform;
                        var movementInput = new Vector3(0, 0, _lastPlayerPos.rotation.z);
                        while (!LookingAtPlayer)
                        {
                              Rotate(movementInput);
                                yield return null;
                        }
                        
                }

                private IEnumerator PerformApproachPlayer()
                {
                        yield return null;
                }

                private IEnumerator PerformIdleMovement()
                {
                        while (!PlayerTooClose )
                        {
                                
                        }
                        yield return null;
                }

                private IEnumerator PerformImpulse()
                {
                        yield return null;
                }

                private IEnumerator PerformRetreat()
                {
                        yield return null;
                }
        }
}