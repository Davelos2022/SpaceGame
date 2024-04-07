using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SpaceGame.Pool
{
    public class ComponentPool<T> : IPool<T> where T : Component
    {
        private readonly Queue<T> _objects = new Queue<T>();
        private readonly GameObject[] _prefabs;
        private readonly Transform _parent;

        private DiContainer _container;

        public ComponentPool(DiContainer diContainer, GameObject[] prefab, int size, Transform parent = null)
        {
            _prefabs = prefab;
            _parent = parent;
            _container = diContainer;

            for (int i = 0; i < size; i++) AddItem();
        }

        private T AddItem()
        {
            GameObject randomPrefab = _prefabs[Random.Range(0, _prefabs.Length)];
            var itemInstance = _container.InstantiatePrefab(randomPrefab, _parent).GetComponent<T>();

            if (itemInstance == null)
            {
                Debug.LogError($"ComponentPool: The instantiated object {randomPrefab.name} does not have a component of type {typeof(T).Name}.");
                return null;
            }

            itemInstance.gameObject.SetActive(false);
            _objects.Enqueue(itemInstance);
            return itemInstance;
        }

        public T Get()
        {
            if (_objects.Count == 0) AddItem();

            T item = _objects.Dequeue();
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            item.gameObject.SetActive(false);
            item.transform.position = Vector3.zero;
            _objects.Enqueue(item);
        }

        public void Clear()
        {
            _objects.Clear();
        }
    }
}