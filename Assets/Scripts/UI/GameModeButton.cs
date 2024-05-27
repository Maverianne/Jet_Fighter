using System;
using Managers;
using UnityEngine;

namespace UI
{
    public class GameModeButton : ModeButtonBase
    {
        
        [SerializeField] private GameModeRecord[] gameModeRecords;
        
        private void Start()
        {
            MaxType = gameModeRecords.Length;
            TMPText.text = gameModeRecords[0].displayModeName;
        }
        protected override void SetMode()
        {
            MainManager.Instance.GameplayManager.CurrentGameMode = (GameplayManager.GameMode)gameModeRecords[CurrentType].mySavingValue;
            TMPText.text = gameModeRecords[CurrentType].displayModeName;
        }
        
        [Serializable]
        private struct GameModeRecord
        {
            public GameplayManager.GameMode myGameMode;
            public string displayModeName;
            public int mySavingValue;
        }
    }
}