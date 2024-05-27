using System.Collections;
using EnemyAI;
using UI;
using UnityEngine;
using UnityEngine.UI;

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
        
        public GameMode CurrentGameMode { get; set; }
        public Difficulty CurrentDifficulty { get; set; }

        public void SetUpGame()
        {
            if(_startedGame) return;
            _startedGame = true;
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
            var sliders = MainManager.Instance.UIManager.Sliders;
            switch (CurrentGameMode)
            {
                case GameMode.Enemy:
                    foreach (var slider in sliders) slider.gameObject.SetActive(false);
                    StartPlayer(_playerOne, sliders[0]);
                    _enemyShip.StartGame();
                    break;
                case GameMode.TwoPlayers:
                    foreach (var slider in sliders) slider.gameObject.SetActive(false);
                    StartPlayer(_playerOne,sliders[0]);
                    StartPlayer(_playerTwo,sliders[1]);
                    break;
            }
        }
        
        
        private void StartPlayer(PlayerController playerController, Slider slider)
        {
            slider.gameObject.SetActive(true);
            playerController.MySlider = slider.GetComponent<ImpulseSlider>();
            playerController.StartGame();
        }
        
        private PlayerController InstantiatePlayer(Vector3 pos, PlayerController checkPlayer, GameObject playerPrefab)
        {
            if (checkPlayer != null) Destroy(checkPlayer.gameObject);
            var newPlayer = Instantiate(playerPrefab);
            var player = newPlayer.GetComponent<PlayerController>();
            player.SetUpSpaceship(pos);
            return player;
        }


        private IEnumerator PerformStartGame(float delay = 1f)
        {
            yield return new WaitForSeconds(delay);
            StartPlayers();
            _startedGame = false;
        }
        private void InstantiateEnemy(Vector3 pos, bool checkNull = true)
        {
            
            if (_enemyShip != null) Destroy(_enemyShip);
            var newEnemy = Instantiate(enemyPrefab);
            var enemy = newEnemy.GetComponent<EnemyShip>();
            enemy.transform.position = pos;
            _enemyShip = enemy;
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