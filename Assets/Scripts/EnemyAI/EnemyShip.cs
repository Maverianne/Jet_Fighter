using System.Collections;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyAI
{
        public class EnemyShip : Spaceship
        {

                private float _zRot;
                private float _lastAttackTimeStamp;
                private float _lastProjectileDodgeTimeStamp;
                private float _currentAttackCoolDown;

                private EnemyBehaviourParameters.EnemyParameters _enemyParameters;
                private PlayerController _player;
                
                private IEnumerator _currentBehaviour; 
                private IEnumerator _previousBehaviour; 
                
                private const string Player = "Player";

                private float DistanceToPlayer => Vector2.Distance(_player.transform.position, transform.position);
                private bool CanPerformOffense => DistanceToPlayer > _enemyParameters.attackDistance;
                private bool CanPerformDefense => DistanceToPlayer < _enemyParameters.retreatDistance;
                private bool CanAttack => Time.unscaledTime > _lastAttackTimeStamp + _currentAttackCoolDown;
                private bool CanCheckForProjectiles => Time.unscaledTime > _lastProjectileDodgeTimeStamp + _enemyParameters.projectileDetectionCoolDown;

                
                protected override void Awake()
                {
                        base.Awake();
                        _player = FindObjectOfType<PlayerController>();
                }
                
                protected override void FixedUpdate()
                {
                        if(!CanPlay) return;
                        MoveSpaceship();
                }
                

                #region SpaceShip custom functions overrides

                public override void StartGame()
                {
                        base.StartGame();
                        SelectNextBehaviour();
                }
                protected override void SetShipParameters()
                {
                        _enemyParameters = MainManager.Instance.EnemyBehaviourParameters.GetBehaviourParameters(MainManager.Instance.GameplayManager.CurrentDifficulty);
                        CurrentSpaceShipParameters = _enemyParameters.shipParameters;
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
                        if(!CanPerformBehaviour(_enemyParameters.impulseChance)) return;
                        base.Impulse();
                }

                protected override void DisableShip()
                {
                        base.DisableShip();
                        MainManager.Instance.GameplayManager.GameRoundDone();
                }

                #endregion
                
                #region Enemy AI
                
                private void SelectNextBehaviour()
                {
                        //Select best behaviour depending on variables
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
                
                private void ForceIdle()
                {
                        ClearCurrentBehaviourSequence(false);
                        _currentBehaviour = PerformIdle(Random.Range(_enemyParameters.minStandbyTime, _enemyParameters.maxStandbyTime));
                        StartCoroutine(_currentBehaviour);
                }
                
              
                private void ForceDodge()
                {
                        //A bullet it's coming towards the enemy
                        //stops the behaviour and prioritizes defense
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
                        
                        if (!CanPerformBehaviour(_enemyParameters.dodgeBulletChance)) return;
                        ForceDodge();
                }

                private bool CanPerformBehaviour(float chance)
                {
                        return Random.Range(0f, 1f) < chance;
                }
                
                 #region Coroutines
                
                //The timers on offense and defense are to make sure the enemy doesn't stay to long in the same behaviour. 
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
                                //If the previous behaviour was offensive, it's assumed the enemy it's looking at the player or tried to
                                //this forces the enemy to look another way, because while on idle it only moves
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
                #endregion

        }
}