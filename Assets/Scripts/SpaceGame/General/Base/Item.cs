using UniRx;
using UnityEngine;
using Zenject;

namespace SpaceGame.General
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Item : MonoBehaviour, IPauseble, IDestroy
    {
        #region Componets
        protected RectTransform RectItem { get; private set; }
        protected BoxCollider2D TriggerItem { get; private set; }
        #endregion

        #region Properties
        protected GameStateManager GameStateManager { get; private set; }
        protected PoolManager PoolManager { get; private set; }
        protected CompositeDisposable Subscriptions { get; set; }
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
        }

        public abstract void PauseGame(bool pause);
        public abstract void Destroy();
        protected abstract void UseItem();
        #endregion
    }
}