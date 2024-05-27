using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup menuBackground;

        [Header("Main Menu")]
        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private Button restartButton;
        [SerializeField] private TMP_Text gameModeText;
        [SerializeField] private TMP_Text gameDifficultyText;
        
        [Header("Game End Menu")]
        [SerializeField] private CanvasGroup endMenu;
        [SerializeField] private TMP_Text endMenuText;
        [SerializeField] private TMP_Text maxScoreText;
        [SerializeField] private TMP_Text minScoreText;

        private CanvasGroup _currentCanvasGroup;
        private Instructions _instructions;
        private PlayerInfo[] _playerInfos;

        private const string Enemy = "Enemy";
        private const string Player = "Player ";
        private const string GameOver = "Game Over";
        private const string Score = " score: ";
        private const string Win = " Won!";
        private const string You = "You ";
        public PlayerInfo[] PlayerInfo => _playerInfos;

        private void Awake()
        {
            _playerInfos = GetComponentsInChildren<PlayerInfo>();
            _instructions = GetComponentInChildren<Instructions>();
            foreach (var info in _playerInfos) info.gameObject.SetActive(false);
            
            _instructions.SetUpInstructions();
            mainMenu.alpha = 0;
            endMenu.alpha = 0;
            SetUpCanvasInteractable(mainMenu, false);
            SetUpCanvasInteractable(endMenu, false);
            SetMenuModes();
        }

        public void StartGame()
        {
            StartCoroutine(PerformFade(0.3f, false, _currentCanvasGroup));
        }

        public void GoToMainMenu()
        {
            endMenu.alpha = 0;
            SetUpCanvasInteractable(endMenu, false);
            _currentCanvasGroup = mainMenu;
            StartCoroutine(PerformFade(0.3f, true, _currentCanvasGroup, false));
        }
        
        public void OpenMainMenu()
        {
            _currentCanvasGroup = mainMenu;
            StartCoroutine(PerformFade(0.3f, true, _currentCanvasGroup));
        }

        private void SetMenuModes()
        {
            MainManager.Instance.GameplayManager.CurrentGameMode = GameplayManager.GameMode.Survival;
            gameModeText.text = MainManager.Instance.GameplayManager.CurrentGameMode.ToString();
            MainManager.Instance.GameplayManager.CurrentDifficulty = GameplayManager.Difficulty.Normal;
            gameDifficultyText.text = MainManager.Instance.GameplayManager.CurrentDifficulty.ToString();
        }

        #region SetScreens
        public void SetGameOver(List<GameplayManager.PlayerScoreRegistry> registries)
        {
            endMenuText.text = GameOver;
            RemoveButton(restartButton);
            StartCoroutine(PerformFade(0.5f, true, endMenu));
            _currentCanvasGroup = endMenu;
            
            maxScoreText.text = string.Empty;
            minScoreText.text = string.Empty;
            //If registry is empty, dont show score
            if(registries.Count == 0) return;
            foreach (var registry in registries)
            {
                if (registry.playerName == Enemy) continue;
                var playerRegistry = registry;
                maxScoreText.text = playerRegistry.playerName + Score + playerRegistry.score;
                break;
            }
        }

        public void SetPlayerWonGame(List<GameplayManager.PlayerScoreRegistry> registries, bool isOnePlayer = false, int winner = 0)
        {
            
            if (isOnePlayer) endMenuText.text = You + Win;
            else endMenuText.text = Player + winner + Win;
            RemoveButton(restartButton, false);
            SetUpScore(registries);
            StartCoroutine(PerformFade(0.5f, true, endMenu));
            _currentCanvasGroup = endMenu;
        }

        private void RemoveButton(Button button, bool remove = true)
        {
            restartButton.interactable = !remove;
            restartButton.gameObject.SetActive(!remove);
        }
        
        private void SetUpScore(List<GameplayManager.PlayerScoreRegistry> registries)
        {
            //Todo: fix score
            maxScoreText.text = string.Empty;
            minScoreText.text = string.Empty;
            //If registry is empty, dont show score
            if(registries.Count == 0) return;
            
            if (MainManager.Instance.GameplayManager.CurrentGameMode == GameplayManager.GameMode.Survival)
            {
                foreach (var registry in registries)
                {
                    if (registry.playerName == Enemy) continue;
                    var playerRegistry = registry;
                    maxScoreText.text = playerRegistry.playerName + Score + playerRegistry.score;
                    break;
                }
            }
            else
            {
                //Set up scores
                var maxScoreRegistry = registries[0];
                var secondMaxScoreRegistry = new GameplayManager.PlayerScoreRegistry();

                var currentMaxScore = 0;
                for (var i = 0; i < registries.Count; i++)
                {
                    if (registries[i].score <= maxScoreRegistry.score) continue;
                    currentMaxScore = i;
                    secondMaxScoreRegistry = maxScoreRegistry;
                    maxScoreRegistry = registries[i];
                }

                if (string.IsNullOrEmpty(secondMaxScoreRegistry.playerName))
                {
                    //if couldn't find another, pick next closes registry
                    currentMaxScore += 1;
                    currentMaxScore = (currentMaxScore + registries.Count) % registries.Count;
                    secondMaxScoreRegistry = registries[currentMaxScore];
                }
                
                maxScoreText.text = maxScoreRegistry.playerName + Score + maxScoreRegistry.score;
                minScoreText.text = secondMaxScoreRegistry.playerName + Score + secondMaxScoreRegistry.score;
            }
        }
        #endregion
        #region Update Canvas
        private void SetUpCanvasInteractable(CanvasGroup canvasGroup, bool interactable)
        {
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;

        }
        private IEnumerator PerformFade(float duration, bool fadeIn, CanvasGroup canvasGroup, bool fadeBackground = true)
        {
            SetUpCanvasInteractable(canvasGroup, fadeIn);
            
            var timer = 0f;
            var startAlpha = canvasGroup.alpha;
            var targetAlpha = fadeIn ? 1 : 0;

            while (timer < duration) {
                timer += Time.deltaTime;
                var progress = Mathf.Clamp01(timer / duration);
                progress = Easing.InOutQuad(progress);
                
                var targetColorAlphaOnly = Mathf.Lerp(startAlpha, targetAlpha, progress);
                
                canvasGroup.alpha = targetColorAlphaOnly;
                if (fadeBackground) menuBackground.alpha = targetColorAlphaOnly;
               
                yield return null;
            }
        }
        #endregion
       
        
        
    }
}