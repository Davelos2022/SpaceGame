using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace SpaceGame.General
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PurpleCrystal : Item, IMoveble
    {
        private bool _isCanMove;

        [SerializeField] private int _valueCrystal;
        [SerializeField] private float _speedMove;

        private void OnEnable()
        {
            GameStateManager.PausedGame += PauseGame;
            Subscriptions = new CompositeDisposable();
            _isCanMove = true;
        }

        private void OnDisable()
        {
            _isCanMove = false;
            GameStateManager.PausedGame -= PauseGame;
            Subscriptions?.Dispose();
        }

        void Start()
        {
            Observable.EveryUpdate()
            .Where(_ => _isCanMove)
            .Subscribe(_ => Move()).AddTo(this);

            TriggerItem.OnTriggerEnter2DAsObservable()
            .Select(other => other.GetComponent<IGiveCrystal>())
             .Where(damageable => damageable != null)
             .Subscribe(damageable =>
              {
                  damageable.GiveCrystal(_valueCrystal);
                  Destroy();
              }).AddTo(Subscriptions);
        }

        public void Move()
        {
            RectItem.position += Vector3.down * _speedMove * Time.deltaTime;
            RectItem.Rotate(new Vector3(0, 0, _speedMove * Time.deltaTime));

            if (RectItem.anchoredPosition.y < -ScreenBorder.HeightBorder)
                Destroy();
        }

        public override void Destroy()
        {
            PoolManager.Return(this);
        }

        public override void PauseGame(bool pause)
        {
            _isCanMove = !pause;
        }

        protected override void UseItem()
        {
            // Future implementation "Item" here.
        }
    }
}