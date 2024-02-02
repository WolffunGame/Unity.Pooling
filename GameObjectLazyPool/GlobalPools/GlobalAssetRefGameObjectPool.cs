using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public class GlobalAssetRefGameObjectPool : IPool, IShareable
    {
        private readonly Dictionary<AssetRefGameObjectPrefab, AssetRefGameObjectItemPool> _pools =
            new(new AssetRefGameObjectPrefabEqualityComparer());

        private readonly Dictionary<int, AssetRefGameObjectPrefab> _prefabToAssetReference = new();

        private readonly Dictionary<AssetReferenceGameObject, AssetRefGameObjectPrefab> _poolKeyCache = new();

        public async UniTask<UnityEngine.GameObject> Rent(AssetReferenceGameObject gameObjectReference)
        {
            if (!_poolKeyCache.TryGetValue(gameObjectReference, out var key))
                _poolKeyCache.Add(gameObjectReference,
                    key = new AssetRefGameObjectPrefab { Source = gameObjectReference, });
            return await Rent(key);
        }

        public async UniTask<UnityEngine.GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference)
        {
            if (!_pools.TryGetValue(gameObjectReference, out var pool))
            {
                pool = new AssetRefGameObjectItemPool(gameObjectReference);
                pool.OnReturn += OnReturnToPool;
                this._pools.Add(gameObjectReference, pool);
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
                Debug.LogError($"GameObject {gameObject.name} is not registered in the pool or was already returned.");
        }

        public void Return(AssetRefGameObjectPrefab gameObjectReference, UnityEngine.GameObject gameObject)
        {
            if (_pools.TryGetValue(gameObjectReference, out var pool))
                pool.Return(gameObject);
        }

        public void ReleaseInstances(int keep, System.Action<UnityEngine.GameObject> onReleased = null)
        {
            foreach (var pool in _pools.Values)
                pool.ReleaseInstances(keep, onReleased);
        }

        private void OnReturnToPool(UnityEngine.GameObject gameObject) => _prefabToAssetReference.Remove(gameObject.GetInstanceID());

        private class AssetRefGameObjectPrefabEqualityComparer : IEqualityComparer<AssetRefGameObjectPrefab>
        {
            public bool Equals([NotNull] AssetRefGameObjectPrefab x, [NotNull] AssetRefGameObjectPrefab y)
                => y is { Source: not null } && x is { Source: not null } &&
                   x.Source.AssetGUID.Equals(y.Source.AssetGUID);
            public int GetHashCode(AssetRefGameObjectPrefab obj) => obj.Source.AssetGUID.GetHashCode();
        }
    }
}