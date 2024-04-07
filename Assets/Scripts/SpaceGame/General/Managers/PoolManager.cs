using UnityEngine;
using System;
using System.Collections.Generic;
using Zenject;
using SpaceGame.Pool;
using System.Reflection;

namespace SpaceGame.General
{
    public class PoolManager : MonoBehaviour
    {
        [Serializable]
        public struct Pool
        {
            public string NameComponent;
            [Range(1, 300)] public int SizePool;
            public GameObject[] Prefabs;
            public Transform Parent;
        }

        [SerializeField] private Pool[] _poolObjects;
        private readonly Dictionary<Type, object> _pools = new Dictionary<Type, object>();
        private DiContainer _container;

        private const string NAMESPACE = "SpaceGame.General";

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        private void Start()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var pool in _poolObjects)
            {
                Type componentType = Type.GetType(NAMESPACE + "." + pool.NameComponent.Trim());

                if (componentType != null)
                {
                    MethodInfo createPoolMethod = typeof(PoolManager).GetMethod(nameof(CreatePool)).MakeGenericMethod(componentType);
                    createPoolMethod.Invoke(this, new object[] { pool.Prefabs, pool.SizePool, pool.Parent });
                }
                else
                {
                    Debug.LogError($"NameComponent: '{pool.NameComponent}' not correction or him not in namespace {NAMESPACE}");
                }

            }
        }

        public void CreatePool<T>(GameObject[] prefab, int size, Transform parent = null) where T : Component
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