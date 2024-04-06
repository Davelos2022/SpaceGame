using Zenject;
using SpaceGame.General;
using SpaceGame.UI;
using UnityEngine;
using SpaceGame.Data;

namespace SpaceGame.MonoInstallers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private EnemiesConfig _enemiesConfig;
        [SerializeField] private AudioData _audioData;

        public override void InstallBindings()
        {
            InstallGeneral();
            InstallUI();
            InstallData();
        }

        private void InstallGeneral()
        {
            Container.Bind<GameStateManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PoolManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<AudioManager>().FromComponentInHierarchy().AsSingle();
            Container.Bind<LevelController>().FromComponentInHierarchy().AsSingle();
        }

        private void InstallUI()
        {
            Container.Bind<HUDController>().FromComponentInHierarchy().AsSingle();
            Container.Bind<PlayerUI>().FromComponentInHierarchy().AsSingle();
        }

        private void InstallData()
        {
            Container.Bind<EnemiesConfig>().FromInstance(_enemiesConfig);
            Container.Bind<AudioData>().FromInstance(_audioData);
        }
    }
}