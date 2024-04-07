using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;
using System;

namespace SpaceGame.General
{
    [RequireComponent(typeof(Image), typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public abstract class SpaceShipBase : MonoBehaviour, IMoveble, IAttack, IDamage, IDestroy, IPauseble, IReset
    {
        #region Components
        public RectTransform RectSpaceShip { get; private set; }
        protected Image ImageBoxSpaceShip { get; private set; }
        protected BoxCollider2D TriggerSpaceShip { get; private set; }
        protected Rigidbody2D RigidbodySpaceShip { get; private set; }
        #endregion

        #region Properties
        [SerializeField] private string _targetNameMask;
        protected GameStateManager GameStateManager { get; private set; }
        protected PoolManager PoolManager { get; private set; }
        protected LevelController LevelController { get; private set; }
        protected AudioManager AudioManager { get; private set; }
        protected Sprite BulletSprite { get; set; }
        protected float Health { get; set; }
        protected float Speed { get; set; }
        protected float Damage { get; set; }
        protected float FireRate { get; set; }
        protected float TimeNextShot { get; set; }
        protected Vector2 DirectionFire { get; set; }
        protected bool IsCanMove { get; set; }
        protected bool IsCanAttack { get; private set; }
        protected CompositeDisposable Subscriptions { get; private set; }
        #endregion

        #region Constants
        private const float CHECK_DELAY = 1f;
        private const float RAY_LENGHT = 40f;
        private const float BOX_SIZE_X = 15f;
        private const float BOX_SIZE_Y = 90;
        #endregion

        #region Methods
        [Inject]
        public void Construct(GameStateManager gameStateManager, PoolManager poolManager, LevelController levelController, AudioManager audioManager)
        {
            GameStateManager = gameStateManager;
            PoolManager = poolManager;
            LevelController = levelController;
            AudioManager = audioManager;
        }

        private void InitializeComponents()
        {
            ImageBoxSpaceShip = GetComponent<Image>();
            RectSpaceShip = GetComponent<RectTransform>();
            TriggerSpaceShip = GetComponent<BoxCollider2D>();
            RigidbodySpaceShip = GetComponent<Rigidbody2D>();
        }

        protected virtual void OptimizationComponents()
        {
            ImageBoxSpaceShip.SetNativeSize();
            TriggerSpaceShip.size = RectSpaceShip.rect.size;
            _targetNameMask = _targetNameMask.Trim();
        }

        protected virtual void Awake()
        {
            InitializeComponents();
        }

        protected virtual void OnEnable()
        {
            GameStateManager.PausedGame += PauseGame;
            GameStateManager.ResetGame += ResetState;
        }

        protected virtual void OnDisable()
        {
            GameStateManager.PausedGame -= PauseGame;
            GameStateManager.ResetGame -= ResetState;
        }

        protected virtual void Start()
        {
            Subscriptions = new CompositeDisposable();

            Observable.EveryFixedUpdate()
           .Where(_ => IsCanMove)
           .Subscribe(_ => Move())
           .AddTo(Subscriptions);

            Observable.Interval(TimeSpan.FromSeconds(CHECK_DELAY))
            .Where(_ => IsCanMove)
           .Subscribe(_ =>
           {
               CheckForEnemies();
           })
           .AddTo(Subscriptions);

            Observable.EveryUpdate()
           .Where(_ => IsCanAttack)
           .Subscribe(_ => Attack())
           .AddTo(Subscriptions);
        }

        private void CheckForEnemies()
        {
            bool hit = Physics2D.BoxCast(RectSpaceShip.position, new Vector2(BOX_SIZE_X, BOX_SIZE_Y), 0,
                DirectionFire, RAY_LENGHT, LayerMask.GetMask(_targetNameMask));

            IsCanAttack = hit ? true : false;
        }

        public void Attack()
        {
            TimeNextShot -= Time.deltaTime;

            if (TimeNextShot <= 0f)
            {
                Vector3 spawnPosition = new Vector3(RectSpaceShip.position.x, RectSpaceShip.position.y + DirectionFire.y, 0);
                Bullet bullet = PoolManager.Get<Bullet>();
                bullet.Init(BulletSprite, spawnPosition, DirectionFire, Damage, _targetNameMask);
                TimeNextShot = 1f / FireRate;

                AudioManager.Play(AudioClipsNames.Shoot);
            }
        }

        public virtual void SetDamage(float damage)
        {
            if (Health <= 0) return;

            AudioManager.Play(AudioClipsNames.Hit);
            EffectHit hit = PoolManager.Get<EffectHit>();
            hit.Init(RectSpaceShip.position);
            Health -= damage;

            if (Health <= 0) Destroy();
        }

        public virtual void Destroy()
        {
            AudioManager.Play(AudioClipsNames.Destroy);
            EffectDestroy effectDestroy = PoolManager.Get<EffectDestroy>();
            effectDestroy.Init(RectSpaceShip.position);
        }

        public virtual void PauseGame(bool pause)
        {
            IsCanMove = !pause;
            IsCanAttack = false;
        }

        public virtual void ResetState()
        {
            Subscriptions?.Dispose();
            IsCanMove = false;
            IsCanAttack = false;
        }

        public abstract void Move();
        #endregion
    }
}