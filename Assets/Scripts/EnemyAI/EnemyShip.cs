using System.Collections;
using UnityEngine;

namespace EnemyAI
{
        public class EnemyShip : Spaceship
        {
                [SerializeField] private EnemyBehaviourParameters enemyBehaviourParameters;

                private float _zRot;
                private float _lastAttackTimeStamp;
                private float _currentAttackCoolDown;

                private IEnumerator _currentBehaviour; 
                
                private const string Player = "Player";

                private float DistanceToPlayer => Vector2.Distance(_player.transform.position, transform.position);
                private bool CanPerformOffense => DistanceToPlayer > _enemyParameters.attackDistance;
                private bool CanPerformDefense => DistanceToPlayer < _enemyParameters.retreatDistance;
                
                private bool CanAttack => Time.unscaledTime > _lastAttackTimeStamp + _currentAttackCoolDown;

                public bool DodgeBullet { get; set; }

                private PlayerController _player;
                private EnemyBehaviourParameters.EnemyParameters _enemyParameters;

                protected override void Awake()
                {
                        base.Awake();
                        _player = FindObjectOfType<PlayerController>();
                }

                protected override void StartGame()
                {
                        base.StartGame();
                        SelectNextBehaviour();
                }


                protected override void SetShipParameters()
                {
                        _enemyParameters = enemyBehaviourParameters.GetBehaviourParameters(EnemyBehaviourParameters.EnemyParameters.Difficulty.Normal);
                        CurrentSpaceShipParameters = _enemyParameters.shipParameters;
                }

                protected override void FixedUpdate() { }
                
                private void SelectNextBehaviour()
                {
                        if (CanPerformOffense)
                        {
                                _currentBehaviour = PerformOffense();
                                StartCoroutine(_currentBehaviour);
                        }
                        else if (CanPerformDefense)
                        {
                                _currentBehaviour = PerformDefense();
                                StartCoroutine(_currentBehaviour);
                        }
                        else
                        {
                                _currentBehaviour = PerformIdle(Random.Range(_enemyParameters.minStandby, _enemyParameters.maxStandby));
                                StartCoroutine(_currentBehaviour);
                        }
                }
                
                protected virtual void ClearCurrentBehaviourSequence(bool selectNextBehaviour = true) 
                {
                        if(_currentBehaviour != null) StopCoroutine(_currentBehaviour);
                        _currentBehaviour = null;
                
                        if(selectNextBehaviour) SelectNextBehaviour();
                }


                private IEnumerator PerformOffense()
                {
                    
                        while (CanPerformOffense)
                        {
                                LookAtPlayer(90);
                                Shooting();
                                yield return null;
                        }
                        ClearCurrentBehaviourSequence();
                }
                
                private IEnumerator PerformDefense()
                {
                        while (CanPerformDefense)
                        {
                                LookAtPlayer(0);
                                yield return null;
                        }

                        Impulse();
                        ClearCurrentBehaviourSequence();
                }

                private IEnumerator PerformIdle(float wait)
                {
                        yield return new WaitForSeconds(wait);
                        SelectNextBehaviour();
                }


                protected override void Shooting()
                {
                        if(!CanAttack) return;
                        
                        var raycastHit = Physics2D.Raycast(ProjectileSpawnPoint.transform.position, -transform.up, 50f, LayerMask.GetMask(Player));
                        if(ReferenceEquals(raycastHit.collider, null)) return;
                        if (!raycastHit.collider.CompareTag(Player)) return;
                        
                        _lastAttackTimeStamp = Time.unscaledTime;
                        _currentAttackCoolDown = Random.Range(_enemyParameters.minAttackCoolDown, _enemyParameters.maxAttackCoolDown);
                        Impulse();
                        base.Shooting();
                }

                protected override void Impulse()
                {
                        if(Random.Range(0f,1f) > _enemyParameters.impulseChance) return;
                        base.Impulse();
                }

                private void LookAtPlayer(float rotationOffset)
                {
                        var position = transform.position;
                        var rotation = transform.rotation;
                        
                        var difference = position - _player.gameObject.transform.position;
                        var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                        _zRot = rotationZ - rotationOffset;
                        
                        var targetRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, _zRot)); ;
                        transform.rotation = Quaternion.RotateTowards(rotation, targetRotation, CurrentSpaceShipParameters.rotatingSpeed * Time.deltaTime * 100);
                }
                
        }
}