using System;
using UniRx;
using UnityEngine;
using SpaceGame.Data;

namespace SpaceGame.General
{
    public class SpaceShipEnemy : SpaceShipBase
    {
        private Vector2 _directionMove;
        private float _displacementMoveY = 350f;
        private bool _changePosition;

        public static Action<int> DestroyEnemy;
        private int _bonusCount;

        public void Init(EnemyData enemy, Vector2 targetMove)
        {
            ImageBoxSpaceShip.sprite = enemy.SpriteEnemy;
            Health = enemy.Health;
            Damage = enemy.Damage;
            Speed = enemy.Speed;
            FireRate = enemy.FireRate;
            BulletSprite = enemy.BulletSprite;
            DirectionFire = Vector2.down;
            IsCanMove = true;

            base.OptimizationComponents();
            base.Start();

            _directionMove = targetMove;
            _bonusCount = enemy.Bonus;
            _changePosition = false;
        }

        public override void Move()
        {
            if (!_changePosition)
            {
                RectSpaceShip.anchoredPosition += _directionMove * Speed * Time.deltaTime;

                if (RectSpaceShip.anchoredPosition.x >= ScreenBorder.WidthOptimaze && _directionMove == Vector2.right
                    || RectSpaceShip.anchoredPosition.x <= -ScreenBorder.WidthOptimaze && _directionMove == Vector2.left)
                    ChangePosition();
            }
        }

        private void ChangePosition()
        {
            _changePosition = true;
            Vector2 startPosition = RectSpaceShip.anchoredPosition;
            Vector2 endPosition = new Vector3(startPosition.x, startPosition.y - _displacementMoveY);

            Observable.EveryUpdate()
            .Where(_ => IsCanMove)
            .TakeWhile(_ => RectSpaceShip.anchoredPosition != endPosition)
            .Subscribe(_ =>
            {
                RectSpaceShip.anchoredPosition = Vector2.MoveTowards(RectSpaceShip.anchoredPosition,
                    endPosition, Speed * Time.deltaTime);
            },
            () =>
            {
                _directionMove = _directionMove == Vector2.right ? Vector2.left : Vector2.right;
                _changePosition = false;
            })
            .AddTo(Subscriptions);
        }

        public override void Destroy()
        {
            DestroyEnemy?.Invoke(_bonusCount);
            base.Destroy();
            ResetState();
        }

        public override void ResetState()
        {
            base.ResetState();
            PoolManager.Return(this);
        }
    }
}