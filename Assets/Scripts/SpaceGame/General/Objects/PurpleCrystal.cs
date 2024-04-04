using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace SpaceGame.General
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class PurpleCrystal : Item
    {
        [SerializeField] private int _valueCrystal;
        [SerializeField] private float _speedMove;

        void Start()
        {
            Observable.EveryUpdate()
            .Where(_ => IsCanMove)
            .Subscribe(_ => Move()).AddTo(this);

            TriggerItem.OnTriggerEnter2DAsObservable()
            .Select(other => other.GetComponent<IGiveCrystal>())
             .Where(damageable => damageable != null)
             .Subscribe(damageable =>
              {
                  damageable.GiveCrystal(_valueCrystal);
                  Destroy();
              }).AddTo(this);
        }

        public override void Move()
        {
            RectItem.position += Vector3.down * _speedMove * Time.deltaTime;
            RectItem.Rotate(new Vector3(0, 0, _speedMove * Time.deltaTime));

            if (RectItem.anchoredPosition.y < -ScreenBorder.HeightOptimimaze)
                Destroy();
        }

        public override void Destroy()
        {
            PoolManager.Return(this);
        }
    }
}