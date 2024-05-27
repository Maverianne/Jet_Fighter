using System.Collections;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private CanvasGroup endMenu;
        [SerializeField] private TMP_Text endMenuText;
        [SerializeField] private TMP_Text gameModeText;
        [SerializeField] private TMP_Text gameDifficultyText;

        private CanvasGroup _currentCanvasGroup;
        private PlayerInfo[] _playerInfos;

        private const string GameOver = "Game Over";
        private const string Win = " Won!";
        private const string You = "You ";
        private const string Player = "Player ";
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

        public void SetGameOver()
        {
            endMenuText.text = GameOver;
            StartCoroutine(PerformFade(0.3f, true, endMenu));
            _currentCanvasGroup = endMenu;
        }

        public void GoToMainMenu()
        {
            StartCoroutine(PerformFade(0.3f, false, _currentCanvasGroup));
            _currentCanvasGroup = mainMenu;
            mainMenu.alpha = 1;
            SetUpCanvasInteractable(mainMenu, true);
        }

        private void SetMenuModes()
        {
          
            MainManager.Instance.GameplayManager.CurrentGameMode = GameplayManager.GameMode.Enemy;
            gameModeText.text = MainManager.Instance.GameplayManager.CurrentGameMode.ToString();
            MainManager.Instance.GameplayManager.CurrentDifficulty = GameplayManager.Difficulty.Normal;
            gameDifficultyText.text = MainManager.Instance.GameplayManager.CurrentDifficulty.ToString();
        }

        private void SetUpCanvasInteractable(CanvasGroup canvasGroup, bool interactable)
        {
            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;

        }
        
        public void SetPlayerWonGame(bool isOnePlayer = false, float playerWinner = 0)
        {
            if (isOnePlayer) endMenuText.text = You + Win;
            else endMenuText.text = Player + playerWinner + Win;
            StartCoroutine(PerformFade(0.3f, true, endMenu));
            _currentCanvasGroup = endMenu;
        }

        private IEnumerator PerformFade(float duration, bool fadeIn, CanvasGroup canvasGroup)
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
               
                yield return null;
            }
        }
        
        
    }
}