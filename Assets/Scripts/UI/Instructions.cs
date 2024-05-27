using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace UI
{
    public class Instructions : MonoBehaviour
    {
        [SerializeField] private CanvasGroup[] slides;
        [SerializeField] private CanvasGroup intructionsCanvasGroup;


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
            if (TerminateSlide)
            {
                
            }
            slides[_currentSlide].alpha = 0;
        }

        private void TerminateInstructions()
        {
            
        }
        private IEnumerator PerformFade(float duration, bool fadeIn, CanvasGroup canvasGroup, bool fadeBackground = true)
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
        }
    }
}
