using UnityEngine;
using Zenject;
using DG.Tweening;
using SpaceGame.Data;
using SpaceGame.UI;

namespace SpaceGame.General
{
    public class SpaceShipPlayer : SpaceShipBase, IGiveCrystal, IGiveAidKit
    {
        private PlayerData _playerData;
        private PlayerUI _playerUI;
        private IInputHandler _inputHandler;

        private const float DURATION_ANIM = .2f;

        [Inject]
        public void Construct(PlayerUI playerUI)
        {
            _playerUI = playerUI;
        }

        protected override void Awake()
        {
            InitController();
            base.Awake();
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

        private void InitController()
        {
            _inputHandler = Application.isMobilePlatform ? new MobileInputHandler() : new PcInputHandler();
        }

        public override void Move()
        {
            float input = _inputHandler.GetInput();
            RigidbodySpaceShip.velocity = new Vector2(input * Speed, RigidbodySpaceShip.velocity.y);
            float clampedX = Mathf.Clamp(RectSpaceShip.anchoredPosition.x, -ScreenBorder.WidthBorder, ScreenBorder.WidthBorder);
            RectSpaceShip.anchoredPosition = new Vector3(clampedX, RectSpaceShip.anchoredPosition.y);
        }

        public override void SetDamage(float damage)
        {
            base.SetDamage(damage);
            _playerUI.SetHealth(Health);
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
            AudioManager.Play(AudioClipsNames.GiveCrystal);
            RectSpaceShip.transform.DOScale(Vector3.one / .8f, DURATION_ANIM).OnComplete(() =>
            {
                RectSpaceShip.transform.DOScale(Vector3.one, DURATION_ANIM).SetAutoKill(true);
                _playerUI.IncreaseCrystal(value);
            });
        }

        public bool GiveAidKit(float value)
        {
            if (Health >= _playerData.Health) return false;


            AudioManager.Play(AudioClipsNames.AidKit);
            Health += value;

            RectSpaceShip.transform.DOShakeScale(DURATION_ANIM).OnComplete(() =>
            {
                _playerUI.SetHealth(Health);
            });

            return true;
        }
    }
}