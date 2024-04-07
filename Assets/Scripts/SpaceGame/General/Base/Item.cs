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

        public void Spawn(Vector2 spawnPosition)
        {
            RectItem.rotation = Quaternion.identity;
            RectItem.anchoredPosition = spawnPosition;
            ActiveItem();
        }

        public virtual void Destroy()
        {
            Subscriptions?.Dispose();
        }

        protected abstract void ActiveItem();
        public abstract void PauseGame(bool pause);
        #endregion
    }
}