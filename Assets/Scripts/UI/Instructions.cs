using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace UI
{
    public class Instructions : MonoBehaviour
    {
        [SerializeField] private CanvasGroup[] slides;

        public void SetUpInstructions()
        {
            foreach (var VARIABLE in SetUpInstructions()) ;
            {
                
            }            
        }

        public void NextSlide()
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
