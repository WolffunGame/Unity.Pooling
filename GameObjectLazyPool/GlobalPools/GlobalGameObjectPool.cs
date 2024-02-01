using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public class GlobalGameObjectPool : IPool, IShareable
    {
        private readonly Dictionary<GameObjectPrefab, GameObjectItemPool> _pools = new();
        
        private readonly Dictionary<UnityEngine.GameObject, GameObjectPrefab> _prefabToAssetReference = new();
        private readonly Dictionary<UnityEngine.GameObject, GameObjectPrefab> _poolKeyCache = new();
        
        public async UniTask<UnityEngine.GameObject> Rent(UnityEngine.GameObject gameObjectReference)
        {
            if (!_poolKeyCache.TryGetValue(gameObjectReference, out var key))
                this._poolKeyCache.Add(gameObjectReference, key = new GameObjectPrefab {
                    Source = gameObjectReference
                });
            return await Rent(key);
        }
        public async UniTask<UnityEngine.GameObject> Rent(GameObjectPrefab gameObjectReference)
        {
            if (!_pools.TryGetValue(gameObjectReference, out var pool))
            {
                if (gameObjectReference.Source.transform.root != null)
                    throw new Exception($"Non Prefab not supported {gameObjectReference.Source.name}");
                pool = new GameObjectItemPool(gameObjectReference);
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
        
        public void Return(GameObjectPrefab gameObjectReference, UnityEngine.GameObject gameObject)
        {
            if (_pools.TryGetValue(gameObjectReference, out var pool))
                pool.Return(gameObject);
        }
        
        public void ReleaseInstances(int keep, Action<UnityEngine.GameObject> onReleased = null)
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