using UnityEngine;
using UnityEngine.UI;
using SpaceGame.Data;
using TMPro;

namespace SpaceGame.UI
{
    [RequireComponent(typeof(Image), typeof(Button))]
    public class CardShip : MonoBehaviour
    {
        [SerializeField] private Image _imageBoxCard;
        [SerializeField] private Button _buttonCard;
        [SerializeField] private TextMeshProUGUI _healthValueText;
        [SerializeField] private TextMeshProUGUI _damageValueText;
        [SerializeField] private TextMeshProUGUI _fireRateValueText;

        public Button ButtonCard { get { return _buttonCard; } set { _buttonCard = value; } }

        public void Init(PlayerData playerData)
        {
            _imageBoxCard.sprite = playerData.SpritePlayer;
            _healthValueText.text = playerData.Health.ToString();
            _damageValueText.text = playerData.Damage.ToString();
            _fireRateValueText.text = playerData.FireRate.ToString();
        }

        private void OnDestroy()
        {
            _buttonCard.onClick.RemoveAllListeners();
        }
    }
}
