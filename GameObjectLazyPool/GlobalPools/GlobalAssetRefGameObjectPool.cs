using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public class GlobalAssetRefGameObjectPool : IPool, IShareable
    {
        private readonly Dictionary<AssetRefGameObjectPrefab, AssetRefGameObjectItemPool> _pools = new();
        
        private readonly Dictionary<UnityEngine.GameObject, AssetRefGameObjectPrefab> _prefabToAssetReference = new();

        private readonly Dictionary<AssetReferenceGameObject, AssetRefGameObjectPrefab> _poolKeyCache = new();
        
        public async UniTask<UnityEngine.GameObject> Rent(AssetReferenceGameObject gameObjectReference)
        {
            if(!_poolKeyCache.TryGetValue(gameObjectReference, out var key))
                _poolKeyCache.Add(gameObjectReference, key = new AssetRefGameObjectPrefab {
                    Source = gameObjectReference,
                });
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
            _prefabToAssetReference.Add(item, gameObjectReference);
            return item;
        }
        
        public void Return(UnityEngine.GameObject gameObject)
        {
            if(!gameObject)
                return;
            if (_prefabToAssetReference.TryGetValue(gameObject, out var assetReference))
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

        private void OnReturnToPool(UnityEngine.GameObject gameObject)
        {
            if(!this._prefabToAssetReference.Remove(gameObject, out var assetReference))
                return;
            if(_poolKeyCache.ContainsKey(assetReference.Source))
                _poolKeyCache.Remove(assetReference.Source);
        } 
    }
}