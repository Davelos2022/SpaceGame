using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SpaceGame.General
{
    [RequireComponent(typeof(Image))]
    public abstract class BulletBase : MonoBehaviour, IMoveble, IDestroy, IPauseble, IReset
    {
        #region Components
        protected RectTransform RectBullet { get; private set; }
        protected Image ImageBoxBullet { get; private set; }
        protected BoxCollider2D TriggetBullet { get; private set; }
        protected CompositeDisposable Subscriptions { get; private set; }
        #endregion

        #region Properties
        protected GameStateManager GameStateManager { get; private set; }   
        protected PoolManager PoolManager { get; private set; }
        protected float DamageBullet { get; set; }
        protected Vector3 Direction { get; set; }
        protected string TargetMask { get; set; }
        private bool _isCanMove;
        #endregion

        #region Constants
        protected const float SPEEED_BULLET = 90f;
        #endregion

        #region Methods
        [Inject]
        public void Construct(GameStateManager gameStateManager, PoolManager poolManager)
        {
            GameStateManager = gameStateManager;
            PoolManager = poolManager;
        }

        private void Awake()
        {
            InitializeComponents();
        }

        protected virtual void OnEnable()
        {
            _isCanMove = true;
            Subscriptions = new CompositeDisposable();

            GameStateManager.PausedGame += PauseGame;
            GameStateManager.ResetGame += ResetState;
        }

        protected virtual void OnDisable()
        {
            _isCanMove = false;
            Subscriptions?.Dispose();

            GameStateManager.PausedGame -= PauseGame;
            GameStateManager.ResetGame -= ResetState;
        }

        protected virtual void Start()
        {
            Observable.EveryUpdate()
            .Where(_ => _isCanMove)
            .Subscribe(_ => Move())
            .AddTo(Subscriptions);

            TriggetBullet.OnTriggerEnter2DAsObservable()
             .Where(colider => colider.gameObject.layer  == LayerMask.NameToLayer(TargetMask))
            .Select(other => other.GetComponent<IDamage>())
             .Where(damageable => damageable != null)
              .Subscribe(damageable =>
               {
                   damageable.SetDamage(DamageBullet);
                   Destroy();
               })
                  .AddTo(Subscriptions);
        }

        private void InitializeComponents()
        {
            ImageBoxBullet = GetComponent<Image>();
            RectBullet = GetComponent<RectTransform>();
            TriggetBullet = GetComponent<BoxCollider2D>();
        }

        protected virtual void OptimizationComponents()
        {
            ImageBoxBullet.SetNativeSize();
        }

        public void PauseGame(bool pause)
        {
            _isCanMove = !pause;
        }

        public abstract void Destroy();
        public abstract void Init(Sprite spriteBullet, Vector3 startPosition, Vector3 targetPositon, float damage, string targetMaskName);
        public abstract void Move();
        public abstract void ResetState();
        #endregion
    }
}