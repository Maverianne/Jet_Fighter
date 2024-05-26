using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyAI
{
        public class EnemyShip : Spaceship
        {
                [SerializeField] private EnemyBehaviourParameters enemyBehaviourParameters;
                
                
                private float _zRot;
                private float _lastAttackTimeStamp;
                private float _lastProjectileDodgeTimeStamp;
                private float _currentAttackCoolDown;

              

                private IEnumerator _currentBehaviour; 
                private IEnumerator _previousBehaviour; 
                
                private const string Player = "Player";

                private float DistanceToPlayer => Vector2.Distance(_player.transform.position, transform.position);
                private bool CanPerformOffense => DistanceToPlayer > _enemyParameters.attackDistance;
                private bool CanPerformDefense => DistanceToPlayer < _enemyParameters.retreatDistance;
                private bool CanAttack => Time.unscaledTime > _lastAttackTimeStamp + _currentAttackCoolDown;
                private bool CanCheckForProjectiles => Time.unscaledTime > _lastProjectileDodgeTimeStamp + _enemyParameters.projectileDetectionCoolDown;


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
                                _currentBehaviour = PerformOffense(2);
                                StartCoroutine(_currentBehaviour);
                        }
                        else if (CanPerformDefense)
                        {
                                _currentBehaviour = PerformDefense(2);
                                StartCoroutine(_currentBehaviour);
                        }
                        else
                        {
                                _currentBehaviour = PerformIdle(Random.Range(_enemyParameters.minStandbyTime, _enemyParameters.maxStandbyTime));
                                StartCoroutine(_currentBehaviour);
                        }
                }
                
                private void ClearCurrentBehaviourSequence(bool selectNextBehaviour = true) 
                {
                        if(_currentBehaviour != null) StopCoroutine(_currentBehaviour);
                        _previousBehaviour = _currentBehaviour;
                        _currentBehaviour = null;
                
                        if(selectNextBehaviour) SelectNextBehaviour();
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

                
                private void ForceIdle()
                {
                        ClearCurrentBehaviourSequence(false);
                        _currentBehaviour = PerformIdle(Random.Range(_enemyParameters.minStandbyTime, _enemyParameters.maxStandbyTime));
                        StartCoroutine(_currentBehaviour);
                }
                
              
                private void ForceDodge()
                {
                        _lastProjectileDodgeTimeStamp = Time.unscaledTime;
                        ClearCurrentBehaviourSequence(false);
                        _currentBehaviour = PerformDefense(2);
                        StartCoroutine(_currentBehaviour);
                }

                private void LookAtPlayer(float rotationOffset)
                {
                        var position = transform.position;
                        var rotation = transform.rotation;
                        
                        var difference = position - _player.gameObject.transform.position;
                        var rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                        _zRot = rotationZ - rotationOffset;
                        
                        var targetRotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, _zRot));
                        transform.rotation = Quaternion.RotateTowards(rotation, targetRotation, CurrentSpaceShipParameters.rotatingSpeed * Time.deltaTime * 100);
                }
                
                private void OnTriggerEnter2D(Collider2D col)
                {
                        if(!CanCheckForProjectiles) return;
                        if (!col.CompareTag(Projectile)) return;
                        var projectileComponent = col.GetComponent<Projectile>();
                        if(MyProjectiles.Contains(projectileComponent)) return;
                        if (!(Random.Range(0f, 1f) < _enemyParameters.dodgeBulletChance)) return;
                        ForceDodge();
                }
                
                #region Coroutines
                
           
                private IEnumerator PerformOffense(float maxDuration)
                {
                        var timer = 0f;
                        while (CanPerformOffense)
                        {
                                timer += Time.deltaTime;
                                LookAtPlayer(90);
                                Shooting();
                                
                                if(timer > maxDuration) ForceIdle();
                                yield return null;
                        }
                        ClearCurrentBehaviourSequence();
                }
                
                private IEnumerator PerformDefense(float maxDuration)
                {
                        var timer = 0f;
                        while (CanPerformDefense)
                        {
                                timer += Time.deltaTime;
                                LookAtPlayer(0);
                                if(timer > maxDuration) ForceIdle();
                                yield return null;
                        }

                        Impulse();
                        ClearCurrentBehaviourSequence();
                }

                private IEnumerator PerformIdle(float wait)
                {

                        if (_previousBehaviour == PerformOffense(2))
                        {
                                yield return PerformRotate(wait /2);
                                yield return new WaitForSeconds(wait /2);
                        }
                        else
                        {
                                yield return new WaitForSeconds(wait);
                        }
                        SelectNextBehaviour();
                }

                private IEnumerator PerformRotate(float rotateDuration)
                {
                        var timer = 0f;
                        while (timer < rotateDuration)
                        {
                                timer += Time.deltaTime;
                                LookAtPlayer(0);
                                yield return null;
                        }
                }
                
                #endregion  

                
        }
}