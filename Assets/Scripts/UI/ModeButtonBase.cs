using TMPro;
using UnityEngine;

namespace UI
{
    public class ModeButtonBase : MonoBehaviour
    {
        
        [SerializeField] private TMP_Text tmpText;

        private TMP_Text _tmpText;
        private float _moveStartTime;
        private int _currentType;

        private const float MoveDelay = 0.1f;

        protected int CurrentType => _currentType;

        protected TMP_Text TMPText => tmpText;
        private bool IsMoving => Time.unscaledTime <= _moveStartTime + MoveDelay;
        
        protected int MaxType { get; set; }
        

        public virtual void PressButton(bool moveRight)
        {
            if (IsMoving) return;
            _moveStartTime = Time.unscaledTime;
            _currentType = moveRight ? _currentType + 1 : _currentType - 1;
            _currentType = (_currentType + MaxType) % MaxType;
            SetMode();
        }

        protected virtual void SetMode()
        {
            
        }
    }
}
