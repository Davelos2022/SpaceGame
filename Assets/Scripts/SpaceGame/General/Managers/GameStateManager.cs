using UnityEngine;
using System;
using Zenject;
using SpaceGame.UI;
using SpaceGame.Data;

namespace SpaceGame.General
{
    public class GameStateManager : MonoBehaviour, IPauseble
    {
        private HUDController _mainUI;
        private PlayerData _currentPlayer;

        public PlayerData CurrentPlayer => _currentPlayer;

        public Action StarGame;
        public Action<bool> PausedGame;
        public Action ResetGame;

        [Inject]
        public void Construct(HUDController mainUI)
        {
            _mainUI = mainUI;
        }

        public void Init(PlayerData currentPlayer)
        {
            _currentPlayer = currentPlayer;
        }

        private void OnEnable()
        {
            _mainUI.ClickPlayGame += PlayGame;
            _mainUI.ClickPause += PauseGame;
            _mainUI.ClickRestart += RestartGame;
            _mainUI.ClickExitMenu += ExitGame;
        }

        private void OnDisable()
        {
            _mainUI.ClickPlayGame -= PlayGame;
            _mainUI.ClickPause -= PauseGame;
            _mainUI.ClickRestart -= RestartGame;
            _mainUI.ClickExitMenu -= ExitGame;
        }

        public void PlayGame()
        {
            StarGame?.Invoke();
        }

        public void PauseGame(bool pause)
        {
            PausedGame?.Invoke(pause);
        }

        public void GameOver()
        {
            PauseGame(true);
            _mainUI.ShowGameOverPanel();
        }

        private void RestartGame()
        {
            PauseGame(false);
            ResetGame?.Invoke();
            PlayGame();
        }

        private void ExitGame()
        {
            PauseGame(false);
            ResetGame?.Invoke();
        }
    }
}