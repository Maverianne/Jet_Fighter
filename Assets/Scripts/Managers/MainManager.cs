using EnemyAI;
using UnityEngine;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {

        [SerializeField] private GameplayParameters gameplayParameters;
        [SerializeField] private EnemyBehaviourParameters enemyBehaviourParameters;
        public static MainManager Instance { get; private set; }
        public GameplayManager GameplayManager { get; private set; }
        public UIManager UIManager { get; private set; }
        
        private bool _isExiting;

        public GameplayParameters GameplayParameters => gameplayParameters;

        public EnemyBehaviourParameters EnemyBehaviourParameters => enemyBehaviourParameters;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            UIManager = FindObjectOfType<UIManager>();
            GameplayManager = GetComponentInChildren<GameplayManager>();
        }
        public void ExitGame()
        {
            if(_isExiting) return;
            _isExiting = true;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
        }
    }
}
