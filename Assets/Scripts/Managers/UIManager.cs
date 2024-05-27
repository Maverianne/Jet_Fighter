using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup menuBackground;

        [Header("Main Menu")]
        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private TMP_Text gameModeText;
        [SerializeField] private TMP_Text gameDifficultyText;
        
        [Header("Game End Menu")]
        [SerializeField] private CanvasGroup endMenu;
        [SerializeField] private TMP_Text endMenuText;
        [SerializeField] private TMP_Text maxScoreText;
        [SerializeField] private TMP_Text minScoreText;

        private CanvasGroup _currentCanvasGroup;
        private PlayerInfo[] _playerInfos;

        private const string Enemy = "Enemy";
        private const string GameOver = "Game Over";
        private const string Score = " score: ";
        private const string Win = " Won!";
        private const string You = "You ";
        public PlayerInfo[] PlayerInfo => _playerInfos;

        private void Awake()
        {
            _playerInfos = GetComponentsInChildren<PlayerInfo>();
            foreach (var info in _playerInfos) info.gameObject.SetActive(false);
            
            mainMenu.alpha = 1;
            endMenu.alpha = 0;
            _currentCanvasGroup = mainMenu;
            SetUpCanvasInteractable(mainMenu, true);
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

        private void SetMenuModes()
        {
            MainManager.Instance.GameplayManager.CurrentGameMode = GameplayManager.GameMode.Enemy;
            gameModeText.text = MainManager.Instance.GameplayManager.CurrentGameMode.ToString();
            MainManager.Instance.GameplayManager.CurrentDifficulty = GameplayManager.Difficulty.Normal;
            gameDifficultyText.text = MainManager.Instance.GameplayManager.CurrentDifficulty.ToString();
        }

        #region SetScreens
        public void SetGameOver()
        {
            endMenuText.text = GameOver;
            StartCoroutine(PerformFade(0.3f, true, endMenu));
            _currentCanvasGroup = endMenu;
        }

       
        public void SetPlayerWonGame(List<GameplayManager.PlayerScoreRegistry> registries, bool isOnePlayer = false, int winner = 0)
        {
            
            if (isOnePlayer) endMenuText.text = You + Win;
            else endMenuText.text = winner + Win;
            SetUpScore(registries);
            StartCoroutine(PerformFade(0.3f, true, endMenu));
            _currentCanvasGroup = endMenu;
        }
        
        private void SetUpScore(List<GameplayManager.PlayerScoreRegistry> registries)
        {
            //Todo: fix score
            maxScoreText.text = string.Empty;
            minScoreText.text = string.Empty;
            if (MainManager.Instance.GameplayManager.CurrentGameMode == GameplayManager.GameMode.Enemy)
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
                GameplayManager.PlayerScoreRegistry maxScoreRegistry = registries[0];
                GameplayManager.PlayerScoreRegistry secondMaxScoreRegistry = new GameplayManager.PlayerScoreRegistry();

                for (var i = 0; i < registries.Count; i++)
                {
                    if (registries[i].score < maxScoreRegistry.score) continue;
                    secondMaxScoreRegistry = maxScoreRegistry;
                    maxScoreRegistry = registries[i];
                }

                if (string.IsNullOrEmpty(secondMaxScoreRegistry.playerName))
                {
                    //if couldn't find another, pick second on registry
                    secondMaxScoreRegistry = registries[1];
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