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

        public async UniTask<UnityEngine.GameObject> Rent(GameObjectPrefab gameObjectReference)
        {
            if (!_pools.TryGetValue(gameObjectReference, out var pool))
            {
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
        
        public void ReleaseInstances(int keep, System.Action<UnityEngine.GameObject> onReleased = null)
        {
            foreach (var pool in _pools.Values)
                pool.ReleaseInstances(keep, onReleased);
        }

        private void OnReturnToPool(UnityEngine.GameObject gameObject) => _prefabToAssetReference.Remove(gameObject);
    }
}