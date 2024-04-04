using UnityEngine;
using UnityEngine.UI;
using System;

namespace SpaceGame.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("MainMenu UI")]
        [SerializeField] private GameObject _mainMenuScreen;
        [SerializeField] private Button _buttonPlay;
        [SerializeField] private Button _buttonExitGame;
        [Header("Pause UI")]
        [SerializeField] private GameObject _pauseScreen;
        [SerializeField] private Button _buttonReturn;
        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonExitInMenu;
        [Header("Game UI")]
        [SerializeField] private GameObject _gameScreen;
        [SerializeField] private Button _buttonPause;
        [Header("Game Over UI")]
        [SerializeField] private GameObject _gameOverScreen;
        [SerializeField] private Button _buttonReplay;

        private GameObject _currentScreen;

        public Action ClickPlayGame;
        public Action<bool> ClickPause;
        public Action ClickRestart;
        public Action ClickExitMenu;

        private void OnEnable()
        {
            _buttonPlay.onClick.AddListener(PlayGameButton);
            _buttonExitGame.onClick.AddListener(ExitGameButton);

            _buttonPause.onClick.AddListener(PauseButton);
            _buttonRestart.onClick.AddListener(ReplayButton);
            _buttonExitInMenu.onClick.AddListener(ExitMenuButton);
            _buttonReturn.onClick.AddListener(PauseButton);

            _buttonReplay.onClick.AddListener(ReplayButton);
        }

        private void OnDisable()
        {
            _buttonPlay.onClick.RemoveAllListeners();
            _buttonExitGame.onClick.RemoveAllListeners();

            _buttonPause.onClick.RemoveAllListeners();
            _buttonRestart.onClick.RemoveAllListeners();
            _buttonExitInMenu.onClick.RemoveAllListeners();

            _buttonReturn.onClick.RemoveAllListeners();
            _buttonReplay.onClick.RemoveAllListeners();
        }

        private void ControllScreen(GameObject screen, bool controll)
        {
            screen.SetActive(controll);
        }

        #region MenuScreen
        private void PlayGameButton()
        {
            ClickPlayGame?.Invoke();
            ControllScreen(_gameScreen, true);
            ControllScreen(_mainMenuScreen, false);
        }

        private void ExitGameButton()
        {
            Application.Quit();
        }
        #endregion

        #region GameScreen
        public void ShowGameOverPanel()
        {
            ControllScreen(_gameOverScreen, true);
        }

        private void PauseButton()
        {
            ClickPause?.Invoke(!_pauseScreen.activeSelf ? true : false);
            ControllScreen(_pauseScreen, !_pauseScreen.activeSelf ? true : false);
        }

        private void ExitMenuButton()
        {
            ClickExitMenu?.Invoke();
            ControllScreen(_pauseScreen, false);
            ControllScreen(_gameScreen, false);
            ControllScreen(_mainMenuScreen, true);
        }

        private void ReplayButton()
        {
            ControllScreen(_pauseScreen, false);
            ControllScreen(_gameOverScreen, false);
            ClickRestart?.Invoke();
        }
        #endregion
    }
}