using UnityEngine;
using System;
using System.Collections.Generic;
using Zenject;
using SpaceGame.Pool;

namespace SpaceGame.General
{
    public class PoolManager : MonoBehaviour
    {
        [Serializable]
        public class Pool
        {
            public int SizePool;
            public TypeObject TypeObject;
            public GameObject[] Prefabs;
            public Transform Parent;
        }

        [SerializeField] private Pool[] _poolObjects;
        private readonly Dictionary<Type, object> _pools = new Dictionary<Type, object>();
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var pool in _poolObjects)
            {
                int randomPrefab = UnityEngine.Random.Range(0, pool.Prefabs.Length);

                switch (pool.TypeObject)
                {
                   
                    case TypeObject.Bullet:
                        CreatePool<Bullet>(pool.Prefabs[randomPrefab], pool.SizePool, pool.Parent);
                        break;
                    case TypeObject.Enemy:
                        CreatePool<SpaceShipEnemy>(pool.Prefabs[randomPrefab], pool.SizePool, pool.Parent);
                        break;
                    case TypeObject.EffectHits:
                        CreatePool<EffectHit>(pool.Prefabs[randomPrefab], pool.SizePool, pool.Parent);
                        break;
                    case TypeObject.EffectDestroy:
                        CreatePool<EffectDestroy>(pool.Prefabs[randomPrefab], pool.SizePool, pool.Parent);
                        break;
                    case TypeObject.Crystal:
                        CreatePool<PurpleCrystal>(pool.Prefabs[randomPrefab], pool.SizePool, pool.Parent);
                        break;
                    default:
                        return;
                }
            }
        }

        public void CreatePool<T>(GameObject prefab, int size, Transform parent = null) where T : Component
        {
            var pool = new ComponentPool<T>(_container, prefab, size, parent);
            _pools.Add(typeof(T), pool);
        }

        public T Get<T>() where T : Component
        {
            if (_pools.TryGetValue(typeof(T), out var poolObject) && poolObject is ComponentPool<T> pool)
            {
                return pool.Get();
            }

            Debug.LogError($"Pool for type {typeof(T)} not found.");
            return null;
        }

        public void Return<T>(T component) where T : Component
        {
            if (_pools.TryGetValue(typeof(T), out var poolObject) && poolObject is ComponentPool<T> pool)
            {
                pool.Return(component);
            }
            else
            {
                Debug.LogError($"Pool for type {typeof(T)} not found. Unable to return component.");
            }
        }
    }
}