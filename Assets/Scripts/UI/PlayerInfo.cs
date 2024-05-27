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
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text score;
        
        public void SetHealthBar(float percentage)
        {
            healthSlider.value = percentage;
        }

        public void SetInfo(int currentScore, string shipName, float resetTime )
        {
            playerName.text = shipName;
            score.text = currentScore.ToString();
            SetHealthBar(1);
            ResetImpulseSlider(resetTime);
        }
        public void ResetImpulseSlider(float resetTime)
        {
            impulseSlider.value = 0;
            StartCoroutine(PerformFillSlider(resetTime));
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
        }
    }
}
