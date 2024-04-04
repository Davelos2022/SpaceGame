using UniRx;
using UnityEngine;
using Zenject;

namespace SpaceGame.General
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Item : MonoBehaviour, IMoveble, IPauseble, IDestroy
    {
        #region Componets
        protected RectTransform RectItem { get; private set; }
        protected BoxCollider2D TriggerItem { get; private set; }
        #endregion

        #region Properties
        protected GameStateManager GameStateManager { get; private set; }
        protected PoolManager PoolManager { get; private set; }
        protected bool IsCanMove { get;  set; }
        protected CompositeDisposable Disposables { get; private set; }
        #endregion

        #region Methods
        [Inject]
        public void Construct(GameStateManager gameManager, PoolManager poolManager)
        {
            GameStateManager = gameManager;
            PoolManager = poolManager;
        }

        private void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            RectItem = GetComponent<RectTransform>();
            TriggerItem = GetComponent<BoxCollider2D>();
        }

        public void Init(Vector2 spawnPosition)
        {
            RectItem.anchoredPosition = spawnPosition;
            IsCanMove = true;
        }

        private void OnEnable()
        {
            GameStateManager.PausedGame += PauseGame;

            Disposables?.Dispose();
            Disposables = new CompositeDisposable();
        }

        private void OnDisable()
        {
            GameStateManager.PausedGame -= PauseGame;
            Disposables?.Dispose();
        }

        public void PauseGame(bool pause)
        {
            IsCanMove = !pause;
        }

        public abstract void Destroy();
        public abstract void Move();
        #endregion
    }
}