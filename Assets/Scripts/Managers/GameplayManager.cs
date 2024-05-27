using System;
using System.Collections;
using System.Collections.Generic;
using EnemyAI;
using UI;
using UnityEngine;

namespace Managers
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] startPositions;
        [SerializeField] private GameObject player01Prefab;
        [SerializeField] private GameObject player02Prefab;
        [SerializeField] private GameObject enemyPrefab;
        
        private bool _startedGame;
        
        private PlayerController _playerOne;
        private PlayerController _playerTwo;
        private EnemyShip _enemyShip;
        private List<PlayerScoreRegistry> _playerScores = new List<PlayerScoreRegistry>();
        
        public GameMode CurrentGameMode { get; set; }
        public Difficulty CurrentDifficulty { get; set; }

        public void SetUpGame( bool newGame)
        {
            if(_startedGame) return;
            _startedGame = true;

            if (newGame)
            {
                _playerScores.Clear();
                _playerScores = new List<PlayerScoreRegistry>();
            }
            
            switch (CurrentGameMode)
            {
                case GameMode.Enemy:
                    _playerOne = InstantiatePlayer(startPositions[0].transform.position, _playerOne, player01Prefab);
                    InstantiateEnemy(startPositions[1].transform.position);
                    break;
                case GameMode.TwoPlayers:
                    _playerOne = InstantiatePlayer(startPositions[0].transform.position, _playerOne, player01Prefab);
                    _playerTwo = InstantiatePlayer(startPositions[1].transform.position, _playerTwo, player02Prefab);
                    break;
            }
            MainManager.Instance.UIManager.StartGame();
            StartCoroutine(PerformStartGame());
        }

        public void GameDone(bool win = true)
        {
            switch (CurrentGameMode)
            {
                case GameMode.Enemy:
                    if (win)
                    {
                        if (_playerOne.CanPlay) _playerOne.FinishedGame();
                        MainManager.Instance.UIManager.SetPlayerWonGame(true);
                    }
                    else
                    {
                        if (_enemyShip.CanPlay) _enemyShip.FinishedGame();
                        MainManager.Instance.UIManager.SetGameOver();
                    }
                        
                    break;
                case GameMode.TwoPlayers:
                    var playerWinner = 0f;
                    if (_playerOne.CanPlay)
                    {
                        _playerOne.FinishedGame();
                        playerWinner = 1;
                    }

                    if (_playerTwo.CanPlay)
                    {
                        _playerTwo.FinishedGame();
                        playerWinner = 2;
                    }
                        
                    MainManager.Instance.UIManager.SetPlayerWonGame(false, playerWinner);
                    break;
            }
        }
        
        
        private void StartPlayers()
        {
            var sliders = MainManager.Instance.UIManager.PlayerInfo;
            switch (CurrentGameMode)
            {
                case GameMode.Enemy:
                    foreach (var slider in sliders) slider.gameObject.SetActive(false);
                    StartSpaceShip(_playerOne, sliders[0]);
                    StartSpaceShip(_enemyShip, sliders[1]);
                    break;
                case GameMode.TwoPlayers:
                    foreach (var slider in sliders) slider.gameObject.SetActive(false);
                    StartSpaceShip(_playerOne,sliders[0]);
                    StartSpaceShip(_playerTwo,sliders[1]);
                    break;
            }
        }
        
        
        private void StartSpaceShip(Spaceship spaceship, PlayerInfo playerInfo)
        {
            playerInfo.gameObject.SetActive(true);
            spaceship.MyStats = playerInfo.GetComponent<PlayerInfo>();
            spaceship.StartGame();
        }
        
        private PlayerController InstantiatePlayer(Vector3 pos, PlayerController checkPlayer, GameObject playerPrefab)
        {
            if (checkPlayer != null) Destroy(checkPlayer.gameObject);
            var newPlayer = Instantiate(playerPrefab);
            var player = newPlayer.GetComponent<PlayerController>();
            player.SetUpSpaceship(pos);
            return player;
        } 
        
        private void InstantiateEnemy(Vector3 pos, bool checkNull = true)
        {
            
            if (_enemyShip != null) Destroy(_enemyShip);
            var newEnemy = Instantiate(enemyPrefab);
            var enemy = newEnemy.GetComponent<EnemyShip>();
            enemy.transform.position = pos;
            _enemyShip = enemy;
        }

        public void AttemptAddData(string playerName, int score)
        {
            var playerRegistry = new PlayerScoreRegistry()
            {
                playerName = playerName,
                score = score,
            };
            for (var i = 0; i < _playerScores.Count; i++)
            {
                if (_playerScores[i].playerName != playerName) continue;
                _playerScores[i] = playerRegistry;
                return;
            }
            _playerScores.Add(playerRegistry);
        }

        public int GetScore(string playerName)
        {
            for (var i = 0; i < _playerScores.Count; i++)
            {
                if (_playerScores[i].playerName != playerName) continue;
                return _playerScores[i].score;
            }

            return 0;
        }
        private IEnumerator PerformStartGame(float delay = 1f)
        {
            yield return new WaitForSeconds(delay);
            StartPlayers();
            _startedGame = false;
        }


        [Serializable]
        public struct PlayerScoreRegistry
        {
            public string playerName;
            public int score;
        }
        
        public enum GameMode
        {
            None = 0, 
            Enemy = 1, 
            TwoPlayers = 2 
        }
        
        public enum Difficulty 
        {   
            None = 0,
            Easy = 1, 
            Normal = 2, 
            Hard = 3
        }
    }
}