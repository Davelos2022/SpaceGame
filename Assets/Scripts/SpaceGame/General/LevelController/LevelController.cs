using UnityEngine;
using UniRx;
using System.Collections;
using Zenject;
using System;
using SpaceGame.Data;

namespace SpaceGame.General
{
    public class LevelController : MonoBehaviour, IPauseble, IReset
    {
        [Header("Move settings")]
        [SerializeField] private RectTransform[] _backGroundLevel;
        [SerializeField] private float _speedMoveLevel;

        [Header("Wave settings")]
        [SerializeField] [Range(2, 5)] private int _minCountEnemy;
        [SerializeField] [Range(5, 10)] private int _maxCountEnemy;
        [SerializeField] [Range(5, 10)] private int _minCountCrystal;
        [SerializeField] [Range(10, 25)] private int _maxCountCrystal;
        [SerializeField] [Range(1f, 2f)] private float _timeToNextCrystal;

        private GameStateManager _gameStateManager;
        private PoolManager _poolManager;
        private AudioManager _audioManager;
        private EnemiesConfig _enemiesConfig;

        private CompositeDisposable _subscriptions;
        private bool _isMoving;

        private int _currentCountEnemy;
        private int _currentWave = 1;
        private const float OFF_SET_SPAWN = 150f;

        private Coroutine _myCoroutine;
        private bool _isActiveEnemies;
        private bool _isPaused;

        public Action<int> Wave;
        public Action ComletedWave;

        public bool isActiveEnemies => _isActiveEnemies;

        [Inject]
        public void Construct(GameStateManager gameStateManager, EnemiesConfig enemiesConfig, PoolManager poolManager, AudioManager audioManager)
        {
            _gameStateManager = gameStateManager;
            _audioManager = audioManager;
            _poolManager = poolManager;
            _enemiesConfig = enemiesConfig;
        }

        private void Awake()
        {
            SetupBackgroundLevel();
        }

        private void OnEnable()
        {
            _gameStateManager.StarGame += ActivateWave;
            _gameStateManager.PausedGame += PauseGame;
            _gameStateManager.ResetGame += ResetState;

            SpaceShipEnemy.DestroyEnemy += (_) => { CountEnemiesControll(); };
        }
        private void OnDisable()
        {
            _gameStateManager.StarGame -= ActivateWave;
            _gameStateManager.PausedGame -= PauseGame;
            _gameStateManager.ResetGame -= ResetState;

            SpaceShipEnemy.DestroyEnemy -= (_) => { CountEnemiesControll(); };
        }

        public void PauseGame(bool pause)
        {
            _isPaused = pause;

            if (!_isActiveEnemies)
                _isMoving = !pause;
        }

        public void ResetState()
        {
            _isMoving = false;
            _subscriptions?.Dispose();
            StopCoroutine(_myCoroutine);
        }

        public void ActivateWave()
        {
            _subscriptions = new CompositeDisposable();
            _myCoroutine = StartCoroutine(StartWave());

            Observable.EveryUpdate()
           .Where(_ => _isMoving)
           .Subscribe(_ => MovingLevel())
            .AddTo(_subscriptions);
        }

        #region Methods for level moving
        private void SetupBackgroundLevel()
        {
            for (int x = 1; x < _backGroundLevel.Length; x++)
            {
                _backGroundLevel[x].anchoredPosition += new Vector2
                    (0, _backGroundLevel[x - 1].anchoredPosition.y + ScreenBorder.HeightScreen);
            }
        }

        private void MovingLevel()
        {
            foreach (var part in _backGroundLevel)
            {
                part.Translate(-Vector3.up * _speedMoveLevel * Time.deltaTime);

                if (part.anchoredPosition.y <= -ScreenBorder.HeightScreen)
                    RepositionPart(part);
            }
        }

        private void RepositionPart(RectTransform part)
        {
            Vector2 groundOffSet = new Vector2(0, (ScreenBorder.HeightScreen * _backGroundLevel.Length));
            part.anchoredPosition = part.anchoredPosition + groundOffSet;
        }
        #endregion

        #region Methods for wave level
        private IEnumerator StartWave()
        {
            _isActiveEnemies = false;
            _isMoving = true;

            int randomCountCrystal = UnityEngine.Random.Range(_minCountCrystal, _maxCountCrystal);

            for (int x = 0; x < randomCountCrystal; x++)
            {
                if (_isPaused)
                {
                    while (_isPaused)
                        yield return null;
                }

                Vector2 randomPosition = new Vector2(UnityEngine.Random.Range(-ScreenBorder.WidthOptimaze, ScreenBorder.WidthOptimaze), ScreenBorder.HeightOptimimaze);
                PurpleCrystal crystal = _poolManager.Get<PurpleCrystal>();
                crystal.Init(randomPosition);

                yield return new WaitForSeconds(_timeToNextCrystal);
            }

            _isMoving = false;
            _subscriptions?.Dispose();
            Wave?.Invoke(_currentWave);

            _isActiveEnemies = true;
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            _currentCountEnemy = UnityEngine.Random.Range(_minCountEnemy, _maxCountEnemy);
            _audioManager.Play(AudioClipsName.SpawnEnemies);

            for (int row = 0; row < _currentCountEnemy; row++)
            {
                Vector3 enemyPosition = new Vector3(0, ScreenBorder.HeightOptimimaze + (row * OFF_SET_SPAWN), 0);

                SpaceShipEnemy spaceShipEnemy = _poolManager.Get<SpaceShipEnemy>();
                spaceShipEnemy.Init(_enemiesConfig.Enemies[UnityEngine.Random.Range(0, _enemiesConfig.Enemies.Length)], GetDirectionalVectorByParity(row));
                spaceShipEnemy.RectSpaceShip.anchoredPosition = enemyPosition;
            }
        }

        private Vector2 GetDirectionalVectorByParity(int number)
        {
            Vector2 newVector = number % 2 == 0 ? Vector2.right : Vector2.left;
            return newVector;
        }

        private void CountEnemiesControll()
        {
            _currentCountEnemy--;

            if (_currentCountEnemy <= 0)
            {
                ComletedWave?.Invoke();
                _currentWave++;
                _isActiveEnemies = false;
                ActivateWave();
            }
        }
        #endregion
    }
}