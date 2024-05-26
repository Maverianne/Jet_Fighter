using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private CanvasGroup endMenu;
        [SerializeField] private TMP_Text endMenuText;

        private CanvasGroup _currentCanvasGroup;
        private Slider[] _sliders;

        private const string GameOver = "Game Over";
        private const string Win = "Won!";
        private const string You = "You!";
        private const string Player = "Player";
        public Slider[] Sliders => _sliders;

        private void Awake()
        {
            _sliders = GetComponentsInChildren<Slider>();
            foreach (var slider in _sliders) slider.gameObject.SetActive(false);
            
            mainMenu.alpha = 1;
            endMenu.alpha = 0;
            _currentCanvasGroup = mainMenu;
            SetUpCanvasInteractable(mainMenu, true);
            SetUpCanvasInteractable(endMenu, false);
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