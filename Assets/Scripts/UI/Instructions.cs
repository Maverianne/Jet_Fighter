using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace UI
{
    public class Instructions : MonoBehaviour
    {
        [SerializeField] private CanvasGroup[] slides;
        [SerializeField] private CanvasGroup instructionsCanvasGroup;


        private bool _changingSlides;
        private bool TerminateSlide => _currentSlide == slides.Length;
        private int _currentSlide;

        public void SetUpInstructions()
        {
            foreach (var slide in slides)
            {
                slide.alpha = 0;
            }

            slides[_currentSlide].alpha = 1;
        }

        public void NextSlide()
        {
            if(_changingSlides) return;
            _changingSlides = true;
            
            var previousSlide = _currentSlide;
            _currentSlide += 1;
            
            
            if (TerminateSlide)
            {
                TerminateInstructions();
                return;
            }
            
            slides[previousSlide].alpha = 0;
            StartCoroutine(PerformFade(0.5f, true, slides[_currentSlide]));
        }

        private void TerminateInstructions()
        {
            StartCoroutine(PerformFade(0.5f, false, instructionsCanvasGroup));
            instructionsCanvasGroup.interactable = false;
            instructionsCanvasGroup.blocksRaycasts = false;
            MainManager.Instance.UIManager.OpenMainMenu();
        }
        private IEnumerator PerformFade(float duration, bool fadeIn, CanvasGroup canvasGroup)
        {
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

            _changingSlides = false;
        }
    }
}
