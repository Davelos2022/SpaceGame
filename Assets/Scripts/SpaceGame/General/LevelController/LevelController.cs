using SpaceGame.Data;
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Zenject;

namespace SpaceGame.General
{
    public class LevelController : MonoBehaviour, IPauseble, IReset
    {
        [Header("Move settings")]
        [SerializeField] private RectTransform[] _backGroundLevel;
        [SerializeField] private float _speedMoveLevel;
        [Space]
        [Header("Wave settings")]
        [SerializeField] [Range(2, 5)] private int _minCountEnemy;
        [SerializeField] [Range(5, 10)] private int _maxCountEnemy;
        [Space]
        [SerializeField] [Range(2f, 5f)] private int _minCountTimeWave;
        [SerializeField] [Range(5f, 20f)] private int _maxCountTimeWave;
        [Space]
        [SerializeField] [Range(1f, 100f)] private float _dropPercentage;
        [SerializeField] [Range(1f, 5f)] private float _checkDelay;

        private GameStateManager _gameStateManager;
        private PoolManager _poolManager;
        private AudioManager _audioManager;
        private EnemiesConfig _enemiesConfig;

        private CompositeDisposable _subscriptions;
        private bool _isMoving;
        private const int MAX_PERCENTAGE = 100;

        private float _timeWave;
        private int _currentCountEnemies;
        private int _currentWave = 1;
        private const float OFF_SET_SPAWN = 150f;

        private Coroutine _myCoroutine;
        private bool _isActiveEnemies;

        public Action<int> Wave;
        public Action CompletedWave;


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
            _timeWave = UnityEngine.Random.Range(_minCountTimeWave, _maxCountTimeWave);
            _isActiveEnemies = false;
            _isMoving = true;

            float tempDelay = _checkDelay;

            while (_timeWave > 0)
            {
                _timeWave -= Time.deltaTime;
                tempDelay -= Time.deltaTime;
                
                if (tempDelay <= 0)
                {
                    DropItem();
                    tempDelay = _checkDelay;
                }
   
                if (!_isMoving)
                {
                    while (!_isMoving)
                        yield return null;
                }

                yield return null;
            }

            _isMoving = false;
            _subscriptions?.Dispose();
            Wave?.Invoke(_currentWave);
            SpawnEnemies();
        }

        private Vector2 GetPositionSpawn()
        {
            Vector2 position = new Vector2(UnityEngine.Random.Range(-ScreenBorder.WidthBorder, ScreenBorder.WidthBorder), ScreenBorder.HeightBorder);
            return position;
        }

        private void DropItem()
        {
            if (UnityEngine.Random.value < (_dropPercentage / MAX_PERCENTAGE))
            {
                Vector2 positionAidKit = GetPositionSpawn();
                Item dropItewm = _poolManager.Get<Item>();
                dropItewm.Spawn(positionAidKit);
            }
        }

        private void SpawnEnemies()
        {
            _currentCountEnemies = UnityEngine.Random.Range(_minCountEnemy, _maxCountEnemy);
            _audioManager.Play(AudioClipsNames.SpawnEnemies);

            for (int row = 0; row < _currentCountEnemies; row++)
            {
                Vector3 enemyPosition = new Vector3(0, ScreenBorder.HeightBorder + (row * OFF_SET_SPAWN), 0);

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
            if (_currentCountEnemies <= 0) return;

            _currentCountEnemies--;

            if (_currentCountEnemies <= 0)
            {
                CompletedWave?.Invoke();
                _currentWave++;
                _isActiveEnemies = false;
                ActivateWave();
            }
        }
        #endregion
    }
}