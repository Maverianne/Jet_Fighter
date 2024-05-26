using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImpulseSlider : MonoBehaviour
    {
        private Slider _mySlider;

        private void Awake()
        {
            _mySlider = GetComponent<Slider>();
            ResetSlider(5);
        }

        public void ResetSlider(float resetTime)
        {
            _mySlider.value = 0;
            StartCoroutine(PerformFillSlider(resetTime));
        }

        private IEnumerator PerformFillSlider(float resetTime)
        {
            var timer = 0f;
            while (timer < resetTime)
            {
                timer += Time.deltaTime;
                _mySlider.value = Mathf.Clamp01(timer / resetTime);
                yield return null;
            }
        }

    }
}
