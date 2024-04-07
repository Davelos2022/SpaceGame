using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SpaceGame.General
{
    public class AidKit : Item, IMoveble, IRotation
    {
        [SerializeField] private int _valueAidKit;
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

        private void StartMove()
        {
            _isCanMove = true;

            Observable.EveryUpdate()
           .Where(_ => _isCanMove)
           .Subscribe(_ => { Move(); Rotation(); }).AddTo(Subscriptions);

            TriggerItem.OnTriggerEnter2DAsObservable()
            .Select(other => other.GetComponent<IGiveAidKit>())
             .Where(aidKit => aidKit != null)
             .Subscribe(aidkit =>
             {
                 if (aidkit.GiveAidKit(_valueAidKit))
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