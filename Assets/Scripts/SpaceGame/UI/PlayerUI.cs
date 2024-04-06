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

        private AccountPlayer _accountPlayer;

        private const string WAVE_TEXT = "Wave";
        private const float DURATION = .2f;

        private void Awake()
        {
            LoadProgress();

        }
        private void LoadProgress()
        {
            _accountPlayer = new AccountPlayer();
            _accountPlayer.InitAccount();
        }

        public void SaveProgress()
        {
            bool initPfrogress = int.TryParse(_pointsCountText.text, out int resultPoints) & int.TryParse(_crystalCountText.text, out int resultCrystals);
            if (initPfrogress) _accountPlayer.Set(resultPoints, resultCrystals);
        }

        public void InitScore()
        {
            _pointsCountText.text = $"{_accountPlayer.CountPoints}";
            _crystalCountText.text = $"{_accountPlayer.CountCrystal}";
        }

        public void InitHealth(float health)
        {
            _sliderHealthPlayer.maxValue = health;
            _sliderHealthPlayer.value = health;
        }

        public void IncreasePoints(int points)
        {
            if (!int.TryParse(_pointsCountText.text, out int currentPoints)) return;

            int result = currentPoints + points;
            _pointsCountText.text = $"{result}";
            AnimUI(_pointsCountText.transform);
        }

        public void IncreaseCrystal(int crystals)
        {
            if (!int.TryParse(_crystalCountText.text, out int currentCrystals)) return;

            int result = currentCrystals + crystals;
            _crystalCountText.text = $"{result}";
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