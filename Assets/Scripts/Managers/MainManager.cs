using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MainManager : MonoBehaviour
    {
        private bool _isExiting;

        public void ExitGame(InputAction.CallbackContext context)
        {
            ExitGame();
        }

        private void ExitGame()
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
