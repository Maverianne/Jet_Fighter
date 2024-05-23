using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainManager : MonoBehaviour
{
    private bool _isExiting;
      
    public void ExitGame(InputAction.CallbackContext context)
    {
        ExitGame();
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
