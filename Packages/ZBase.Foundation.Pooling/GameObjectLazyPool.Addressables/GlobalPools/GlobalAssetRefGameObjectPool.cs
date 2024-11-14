using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool
{
    public class GlobalAssetRefGameObjectPool : IPool, IShareable
    {
        private readonly Dictionary<AssetRefGameObjectPrefab, AssetRefGameObjectItemPool> _pools = new(new AssetRefGameObjectPrefabEqualityComparer());
        private readonly Dictionary<int, AssetRefGameObjectItemPool> _dicTrackingInstancePools = new();
        private readonly Dictionary<AssetReferenceGameObject, AssetRefGameObjectPrefab> _poolKeyCache = new();
        private readonly Dictionary<string, AssetRefGameObjectPrefab> _poolStringKeyCache = new();
        
        public async UniTask<GameObject> Rent(string address)
        {
            if (_poolStringKeyCache.TryGetValue(address, out var key))
                return await Rent(key);
            var assetRef = new AssetReferenceGameObject(address);
            this._poolStringKeyCache.Add(address, key = new AssetRefGameObjectPrefab { Source = assetRef, });
            return await Rent(key);
        }

        public async UniTask<GameObject> Rent(AssetReferenceGameObject gameObjectReference)
        {
            if (_poolKeyCache.TryGetValue(gameObjectReference, out var key))
                return await Rent(key);
            key = new AssetRefGameObjectPrefab { Source = gameObjectReference };
            this._poolKeyCache[gameObjectReference] = key;
            return await Rent(key);
        }

        public async UniTask<GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference)
        {
            if (!_pools.TryGetValue(gameObjectReference, out var pool))
            {
                pool = new AssetRefGameObjectItemPool(gameObjectReference);
                pool.OnItemDestroyAction += RemoveTrackingItem;
                pool.OnReturnAction += RemoveTrackingItem;
                this._pools.Add(gameObjectReference, pool);
            }
            var item = await pool.Rent();
            var keyInstance = item.GetInstanceID();
            if (_dicTrackingInstancePools.ContainsKey(keyInstance))
                Debug.LogError($"Duplicate key pool {gameObjectReference.Source.SubObjectName}");
            _dicTrackingInstancePools[keyInstance]= pool;
            return item;
        }

        public void Return(GameObject gameObject)
        {
            if (!gameObject)
                return;
            if (_dicTrackingInstancePools.TryGetValue(gameObject.GetInstanceID(), out var pool))
                pool.Return(gameObject);
            else
                Debug.LogWarning($"GameObject {gameObject.name} is not registered in the pool or was already returned.");
        }

        public void Return(AssetRefGameObjectPrefab gameObjectReference, GameObject gameObject)
        {
            if (_pools.TryGetValue(gameObjectReference, out var pool))
                pool.Return(gameObject);
        }

        public void ReleaseInstances(int keep, System.Action<GameObject> onReleased = null)
        {
            foreach (var pool in _pools.Values)
                pool.ReleaseInstances(keep, onReleased);
        }

        private void RemoveTrackingItem(GameObject gameObject) => _dicTrackingInstancePools.Remove(gameObject.GetInstanceID());

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
                pool.Dispose();
            _pools.Clear();
            _dicTrackingInstancePools.Clear();
            _poolKeyCache.Clear();
            _poolStringKeyCache.Clear();
        }
        
        private class AssetRefGameObjectPrefabEqualityComparer : IEqualityComparer<AssetRefGameObjectPrefab>
        {
            public bool Equals([NotNull] AssetRefGameObjectPrefab x, [NotNull] AssetRefGameObjectPrefab y)
                => y is { Source: not null } && x is { Source: not null } &&
                   x.Source.AssetGUID.Equals(y.Source.AssetGUID);
            public int GetHashCode(AssetRefGameObjectPrefab obj) => obj.Source.AssetGUID.GetHashCode();
        }
    }
}