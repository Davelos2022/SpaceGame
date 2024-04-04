using UnityEngine;
using UnityEngine.UI;
using System;

namespace SpaceGame.UI
{
    public class HUDController : MonoBehaviour
    {
        [Header("UI Screens")]
        [SerializeField] private GameObject _mainMenuScreen;
        [SerializeField] private GameObject _pauseScreen;
        [SerializeField] private GameObject _gameScreen;
        [SerializeField] private GameObject _gameOverScreen;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button _buttonPlay;
        [SerializeField] private Button _buttonExitGame;

        [Header("In-Game Buttons")]
        [SerializeField] private Button _buttonPause;

        [Header("Pause Screen Buttons")]
        [SerializeField] private Button _buttonReturn;
        [SerializeField] private Button _buttonRestart;
        [SerializeField] private Button _buttonExitInMenu;

        [Header("Game Over Screen Buttons")]
        [SerializeField] private Button _buttonReplay;
        [SerializeField] private Button _buttonExit;

        public Action ClickPlayGame;
        public Action<bool> ClickPause;
        public Action ClickRestart;
        public Action ClickExitMenu;

        private void Awake()
        {
            SetupButtonListeners();
            SwitchScreen(_mainMenuScreen);
        }

        private void SetupButtonListeners()
        {
            _buttonPlay.onClick.AddListener(PlayButton);
            _buttonExitGame.onClick.AddListener(Application.Quit);
            _buttonPause.onClick.AddListener(() => PauseButton());
            _buttonReturn.onClick.AddListener(() => PauseButton());
            _buttonRestart.onClick.AddListener(ReplayGame);
            _buttonExitInMenu.onClick.AddListener(() => ExitToMainMenu(true));
            _buttonReplay.onClick.AddListener(ReplayGame);
            _buttonExit.onClick.AddListener(() => ExitToMainMenu(false));
        }


        private void SwitchScreen(GameObject screenToShow)
        {
            _mainMenuScreen.SetActive(false);
            _gameScreen.SetActive(false);
            _pauseScreen.SetActive(false);
            _gameOverScreen.SetActive(false);

            screenToShow.SetActive(true);
        }

        private void PlayButton()
        {
            ClickPlayGame?.Invoke();
            SwitchScreen(_gameScreen);
        }

        private void PauseButton()
        {
            bool isPausing = !_pauseScreen.activeSelf;
            _pauseScreen.SetActive(isPausing);
            ClickPause?.Invoke(isPausing);
        }

        private void ReplayGame()
        {
            ClickRestart?.Invoke();
            SwitchScreen(_gameScreen);
        }


        private void ExitToMainMenu(bool fromPauseScreen)
        {
            if (fromPauseScreen) ClickExitMenu?.Invoke();
            SwitchScreen(_mainMenuScreen);
        }

        public void ShowGameOverPanel()
        {
            SwitchScreen(_gameOverScreen);
        }
    }
}