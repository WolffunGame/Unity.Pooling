using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public class GlobalGameObjectPool : IPool, IShareable
    {
        private readonly Dictionary<int, GameObjectItemPool> _pools = new();
        private readonly Dictionary<int, GameObjectPrefab> _prefabToAssetReference = new();
        private readonly Dictionary<int, GameObjectPrefab> _poolKeyCache = new();

        public async UniTask<UnityEngine.GameObject> Rent(UnityEngine.GameObject gameObjectReference)
        {
            var hash = gameObjectReference.GetInstanceID();
            if (!_poolKeyCache.TryGetValue(hash, out var key))
                this._poolKeyCache.Add(hash, key = new GameObjectPrefab { Source = gameObjectReference });
            return await Rent(key);
        }

        public async UniTask<UnityEngine.GameObject> Rent(GameObjectPrefab gameObjectReference)
        {
            var instanceID = gameObjectReference.Source.GetInstanceID();
            if (!_pools.TryGetValue(instanceID, out var pool))
            {
                if (gameObjectReference.Source.transform.root != gameObjectReference.Source.transform)
                    throw new Exception($"Non Prefab not supported {gameObjectReference.Source.name}");
                pool = new GameObjectItemPool(gameObjectReference);
                pool.OnReturn += OnReturnToPool;
                this._pools.Add(instanceID, pool);
            }

            UnityEngine.GameObject item = await pool.Rent();
            _prefabToAssetReference.Add(item.GetInstanceID(), gameObjectReference);
            return item;
        }

        public void Return(UnityEngine.GameObject gameObject)
        {
            if (!gameObject)
                return;
            if (_prefabToAssetReference.TryGetValue(gameObject.GetInstanceID(), out var assetReference))
                Return(assetReference, gameObject);
            else
                Debug.LogWarning($"GameObject {gameObject.name} is not registered in the pool or was already returned.");
        }

        public void Return(GameObjectPrefab gameObjectReference, UnityEngine.GameObject gameObject)
        {
            if (_pools.TryGetValue(gameObjectReference.Source.GetInstanceID(), out var pool))
                pool.Return(gameObject);
        }

        public void ReleaseInstances(int keep, Action<UnityEngine.GameObject> onReleased = null)
        {
            foreach (var pool in _pools.Values)
                pool.ReleaseInstances(keep, onReleased);
        }

        private void OnReturnToPool(UnityEngine.GameObject gameObject) => _prefabToAssetReference.Remove(gameObject.GetInstanceID());
    }
}