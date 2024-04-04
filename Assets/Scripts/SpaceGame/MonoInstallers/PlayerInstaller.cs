using Zenject;
using UnityEngine;
using SpaceGame.Data;

namespace SpaceGame.MonoInstalllers
{
    public class PlayerInstaller : MonoInstaller
    {
        [SerializeField] private PlayerConfig _playerConfig;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private RectTransform _parentPlayer;
        [SerializeField] private Transform _startPosition;

        public override void InstallBindings()
        {
            InstallPlayer();
        }

        private void InstallPlayer()
        {
            Container.Bind<PlayerConfig>().FromInstance(_playerConfig);
        }

        public override void Start()
        {
            GameObject player = Container.InstantiatePrefab(_playerPrefab, _startPosition.position, Quaternion.identity, _parentPlayer);
            player.transform.localScale = Vector3.one;
        }
    }
}
