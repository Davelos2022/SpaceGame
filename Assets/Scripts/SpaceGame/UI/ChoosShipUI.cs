using UnityEngine;
using UnityEngine.UI;
using Zenject;
using SpaceGame.Data;
using SpaceGame.General;

namespace SpaceGame.UI
{
    public class ChoosShipUI : MonoBehaviour
    {
        [SerializeField] private GameObject _cardPrefab;
        [SerializeField] private Transform _parentContent;
        [SerializeField] private GameObject _selectionObject;
        [SerializeField] private Button _playButton;

        private PlayerConfig _playerConfig;
        private GameStateManager _gameManager;
        private AudioManager _audioManager;
        //private Transform _parrentSelection;

        [Inject]
        public void Construct (PlayerConfig playerConfig, GameStateManager gameManager, AudioManager audioManager)
        {
            _playerConfig = playerConfig;
            _gameManager = gameManager;
            _audioManager = audioManager;
        }

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            //_parrentSelection = _selectionObject.transform.parent;
            _selectionObject.gameObject.SetActive(false);
            _playButton.interactable = false;

            foreach(var card in _playerConfig.Players)
            {
                GameObject cardObj = Instantiate(_cardPrefab, _parentContent);
                CardShip cardShip = cardObj.GetComponent<CardShip>();
                cardShip.ButtonCard.onClick.AddListener(() => Selection(cardShip.ButtonCard, card));
                cardShip.Init(card);
            }
        }

        private void Selection(Button button, PlayerData currentPlayer)
        {
            _audioManager.Play(AudioClipsNames.Selection);
            _selectionObject.transform.position = button.transform.position;
            _selectionObject.transform.SetParent(button.transform);
            _selectionObject.SetActive(true);

            _playButton.interactable = _selectionObject.activeInHierarchy;
            _gameManager.Init(currentPlayer);
        }

        //private void ClearSelection()
        //{
        //    _selectionObject.transform.SetParent(_parrentSelection);
        //    _selectionObject.SetActive(false);
        //    _playButton.interactable = false;
        //}
    }
}