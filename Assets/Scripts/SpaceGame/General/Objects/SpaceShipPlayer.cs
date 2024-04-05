using UnityEngine;
using Zenject;
using DG.Tweening;
using SpaceGame.Data;
using SpaceGame.UI;

namespace SpaceGame.General
{
    public class SpaceShipPlayer : SpaceShipBase, IGiveCrystal
    {
        private PlayerData _playerData;
        private PlayerUI _playerUI;

        private const float DURATION_ANIM = .2f;
        private const float SENSITIVY_FACTOR = 80f; // Sensitivity factor to adjust the effect of mouse movement

        [Inject]
        public void Construct(PlayerUI playerUI)
        {
            _playerUI = playerUI;
        }

        protected override void Start()
        {
            GameStateManager.StarGame += Init;
            LevelController.Wave += _playerUI.ShowNumberWave;
            LevelController.CompletedWave += _playerUI.SaveProgress;
            SpaceShipEnemy.DestroyEnemy += _playerUI.IncreasePoints;
        }

        private void OnDestroy()
        {
            GameStateManager.StarGame -= Init;
            LevelController.Wave -= _playerUI.ShowNumberWave;
            LevelController.CompletedWave -= _playerUI.SaveProgress;
            SpaceShipEnemy.DestroyEnemy -= _playerUI.IncreasePoints;
        }

        public void Init()
        {
            _playerData = GameStateManager.CurrentPlayer;

            ImageBoxSpaceShip.sprite = _playerData.SpritePlayer;
            Health = _playerData.Health;
            Speed = _playerData.Speed;
            Damage = _playerData.Damage;
            FireRate = _playerData.FireRate;
            BulletSprite = _playerData.BulletSprite;
            DirectionFire = Vector2.up;
            IsCanMove = true;

            base.Start();
            base.OptimizationComponents();

            _playerUI.InitScore();
            _playerUI.InitHealth(_playerData.Health);
            RectSpaceShip.gameObject.SetActive(true);
        }

        private float GetInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    return touch.deltaPosition.x;
                }
            }
            else if (Input.GetMouseButton(0))
            {
                return Input.GetAxis("Mouse X") * SENSITIVY_FACTOR;
            }

            return 0f;
        }

        public override void Move()
        {
            RigidbodySpaceShip.velocity = new Vector2(GetInput() * Speed, RigidbodySpaceShip.velocity.y);
            float clampedX = Mathf.Clamp(RectSpaceShip.anchoredPosition.x, -ScreenBorder.WidthBorder, ScreenBorder.WidthBorder);
            RectSpaceShip.anchoredPosition = new Vector3(clampedX, RectSpaceShip.anchoredPosition.y);
        }

        public override void SetDamage(float damage)
        {
            _playerUI.SetHealth(damage);
            base.SetDamage(damage);
        }

        public override void Destroy()
        {
            GameStateManager.GameOver();
            base.Destroy();
            ResetState();
        }

        public override void ResetState()
        {
            RectSpaceShip.gameObject.SetActive(false);
            base.ResetState();
        }

        public void GiveCrystal(int value)
        {
            AudioManager.Play(AudioClipsName.GiveCrystal);
            RectSpaceShip.transform.DOScale(Vector3.one / .8f, DURATION_ANIM).OnComplete(() =>
            {
                RectSpaceShip.transform.DOScale(Vector3.one, DURATION_ANIM).SetAutoKill(true);
                _playerUI.IncreaseCrystal(value);
            });
        }
    }
}