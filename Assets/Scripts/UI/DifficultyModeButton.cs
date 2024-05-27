using System;
using Managers;
using UnityEngine;

namespace UI
{
    public class DifficultyModeButton : ModeButtonBase
    {
        [SerializeField] private DifficultyRecord[] difficultyRecords;
        private bool CanPressButton => MainManager.Instance.GameplayManager.CurrentGameMode == GameplayManager.GameMode.Enemy;

        private void Start()
        {
            MaxType = difficultyRecords.Length;
        }
        
        public override void PressButton(bool moveRight)
        {
            if(!CanPressButton) return;
            base.PressButton(moveRight);
            
        }

        protected override void SetMode()
        {
            MainManager.Instance.GameplayManager.CurrentDifficulty = (GameplayManager.Difficulty) difficultyRecords[CurrentType].mySavingValue;
            TMPText.text = difficultyRecords[CurrentType].displayModeName;;
        }
        
        [Serializable]
        private struct DifficultyRecord
        {
            public GameplayManager.Difficulty myDifficulty;
            public string displayModeName;
            public int mySavingValue;
        }
    }
}