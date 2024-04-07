using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace SpaceGame.General
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Crystal : Item, IMoveble, IRotation
    {
        [SerializeField] private int _valueCrystal;
        [SerializeField] private float _speedMove;
        [SerializeField] private float _speedRotation;

        private bool _isCanMove;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void ActiveItem()
        {
            base.ActiveItem();
            StartMove();
        }

        public void StartMove()
        {
            Subscriptions = new CompositeDisposable();
            _isCanMove = true;

            Observable.EveryUpdate()
           .Where(_ => _isCanMove)
           .Subscribe(_ => { Move(); Rotation(); }).AddTo(Subscriptions);

            TriggerItem.OnTriggerEnter2DAsObservable()
            .Select(other => other.GetComponent<IGiveCrystal>())
             .Where(crystal => crystal != null)
             .Subscribe(crystal =>
             {
                 crystal.GiveCrystal(_valueCrystal);
                 Destroy();
             }).AddTo(Subscriptions);
        }

        public void Move()
        {
            RectItem.position += Vector3.down * _speedMove * Time.deltaTime;

            if (RectItem.anchoredPosition.y < -ScreenBorder.HeightBorder)
                Destroy();
        }

        public void Rotation()
        {
            RectItem.Rotate(new Vector3(0, 0, _speedRotation * Time.deltaTime));
        }

        public override void PauseGame(bool pause)
        {
            _isCanMove = !pause;
        }

        public override void Destroy()
        {
            base.Destroy();
        }
    }
}