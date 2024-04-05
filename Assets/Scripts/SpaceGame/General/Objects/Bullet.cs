using UnityEngine;

namespace SpaceGame.General
{
    public class Bullet : BulletBase
    {
        public override void Init(Sprite spriteBullet, Vector3 startPosition, Vector3 targetPosition, float damage, string targetMaskName)
        {
            ImageBoxBullet.sprite = spriteBullet;
            DamageBullet = damage;
            RectBullet.position = startPosition;
            Direction = targetPosition;
            TargetMask = targetMaskName;

            base.Start();
            base.OptimizationComponents();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        public override void Move()
        {
            if (RectBullet.anchoredPosition.y > ScreenBorder.HeightScreen || RectBullet.anchoredPosition.y < -ScreenBorder.WidthScreen)
                PoolManager.Return(this);
            else
                RectBullet.position += Direction * SPEED_BULLET * Time.deltaTime;
        }

        public override void Destroy()
        {
            ResetState();
        }

        public override void ResetState()
        {
            PoolManager.Return(this);
        }
    }
}