using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.ItemPooling;

namespace ZBase.Foundation.Pooling.AddressableAssets
{
    public class GlobalAssetRefGameObjectPool : IPool, IShareable
    {
        private readonly Dictionary<AssetRefGameObjectPrefab, AssetRefGameObjectItemPool> _pools = new();
        
        private readonly Dictionary<GameObject, AssetRefGameObjectPrefab> _prefabToAssetReference = new();

        public async UniTask<GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference)
        {
            if (!_pools.TryGetValue(gameObjectReference, out var pool))
            {
                pool = new AssetRefGameObjectItemPool(gameObjectReference);
                pool.OnReturn += OnReturnToPool;
                this._pools.Add(gameObjectReference, pool);
            }
            GameObject item = await pool.Rent();
            _prefabToAssetReference.Add(item, gameObjectReference);
            return item;
        }
        
        public void Return(GameObject gameObject)
        {
            if(!gameObject)
                return;
            if (_prefabToAssetReference.TryGetValue(gameObject, out var assetReference))
                Return(assetReference, gameObject);
            else
                Debug.LogError($"GameObject {gameObject.name} is not registered in the pool");
        }
        
        public void Return(AssetRefGameObjectPrefab gameObjectReference, GameObject gameObject)
        {
            if (_pools.TryGetValue(gameObjectReference, out var pool))
                pool.Return(gameObject);
        }

        private void OnReturnToPool(GameObject gameObject) => _prefabToAssetReference.Remove(gameObject);
    }
}