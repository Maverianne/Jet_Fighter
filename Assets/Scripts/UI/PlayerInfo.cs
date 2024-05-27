using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider impulseSlider;
        [SerializeField] private CanvasGroup scoreCg;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text score;

        private IEnumerator _fillImpulseSlider;

        public bool CanImpulse { get; private set; }
        
        public void SetHealthBar(float percentage)
        {
            healthSlider.value = percentage;
        }

        public void SetInfo(int currentScore, string shipName, float resetTime, bool isPlayer = true )
        {
            playerName.text = shipName;
            if (isPlayer) score.text = currentScore.ToString();
            var scoreAlpha = isPlayer ? 1 : 0;
            scoreCg.alpha = scoreAlpha;
            SetHealthBar(1);
            CanImpulse = true;
            ResetImpulseSlider(resetTime);
        }
        public void ResetImpulseSlider(float resetTime)
        {
            if(!CanImpulse) return;
            CanImpulse = false;
            impulseSlider.value = 0;
            _fillImpulseSlider = PerformFillSlider(resetTime);
            StartCoroutine(_fillImpulseSlider);
        }

        private void OnDisable()
        {
            if(_fillImpulseSlider != null) StopCoroutine(_fillImpulseSlider);
        }

        private IEnumerator PerformFillSlider(float resetTime)
        {
            var timer = 0f;
            while (timer < resetTime)
            {
                timer += Time.deltaTime;
                impulseSlider.value = Mathf.Clamp01(timer / resetTime);
                yield return null;
            }

            CanImpulse = true;
        }
    }
}
