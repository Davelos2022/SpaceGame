using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using SpaceGame.ProgressGame;

namespace SpaceGame.UI
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _pointsCountText;
        [SerializeField] private TextMeshProUGUI _crystalCountText;
        [SerializeField] private TextMeshProUGUI _textWave;
        [SerializeField] private Slider _sliderHealthPlayer;

        private SaveLoadProgress _progressPlayer;
        private int _currentPoints;
        private int _currentCystals;

        public int CurrentPoint => _currentPoints;
        public int CurrentCoins => _currentCystals;

        private const string WAVE_TEXT = "Wave";
        private const float DURATION = .2f;

        private void LoadProgress()
        {
            _progressPlayer = new SaveLoadProgress();
            _currentPoints = _progressPlayer.LoadPointProgress();
            _currentCystals = _progressPlayer.LoadCoinsProgress();
        }

        public void SaveProgress()
        {
            _progressPlayer.SaveProgress(_currentPoints, _currentCystals);
        }

        public void InitScore()
        {
            LoadProgress();
            _pointsCountText.text = $"{_currentPoints}";
            _crystalCountText.text = $"{_currentCystals}";
        }

        public void InitHealth(float health)
        {
            _sliderHealthPlayer.maxValue = health;
            _sliderHealthPlayer.value = health;
        }

        public void IncreasePoints(int points)
        {
            _currentPoints += points;
            _pointsCountText.text = $"{_currentPoints}";

            AnimUI(_pointsCountText.transform);
        }

        public void IncreaseCrystal(int coins)
        {
            _currentCystals += coins;
            _crystalCountText.text = $"{_currentCystals}";

            AnimUI(_crystalCountText.transform);
        }

        public void SetHealth(float damage)
        {
            if (_sliderHealthPlayer.value <= 0) return;
            _sliderHealthPlayer.value -= damage;
        }

        public void ShowNumberWave(int currentWave)
        {
            _textWave.text = $"{WAVE_TEXT} {currentWave}";
            _textWave.gameObject.SetActive(true);

            AnimUI(_textWave.transform, true);
        }

        private void AnimUI(Transform transform, bool deactiveUI = false)
        {
            transform.DOScale(Vector3.one / .7f, DURATION).OnComplete(() =>
            {
                transform.DOScale(Vector3.one, DURATION).OnComplete(() => 
                {
                    transform.gameObject.SetActive(deactiveUI ? false : true);

                }).SetAutoKill(true);

            });
        }
    }
}