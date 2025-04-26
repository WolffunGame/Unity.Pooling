using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
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
        private SemaphoreSlim _semaphore;
        private bool _isDisposed;

        public GlobalAssetRefGameObjectPool()
        {
            _semaphore = new SemaphoreSlim(1, 1);
        }

        public async UniTask<GameObject> Rent(string address)
        {
            ThrowIfDisposed();
            
            try
            {
                await _semaphore.WaitAsync();
                if (_poolStringKeyCache.TryGetValue(address, out var key))
                {
                    _semaphore.Release();
                    return await Rent(key);
                }
                
                var assetRef = new AssetReferenceGameObject(address);
                key = new AssetRefGameObjectPrefab { Source = assetRef };
                _poolStringKeyCache.Add(address, key);
                _semaphore.Release();
                
                return await Rent(key);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error renting from address {address}: {ex.Message}");
                _semaphore?.Release();
                throw;
            }
        }

        public async UniTask<GameObject> Rent(AssetReferenceGameObject gameObjectReference)
        {
            ThrowIfDisposed();
            
            try
            {
                await _semaphore.WaitAsync();
                if (_poolKeyCache.TryGetValue(gameObjectReference, out var key))
                {
                    _semaphore.Release();
                    return await Rent(key);
                }
                
                key = new AssetRefGameObjectPrefab { Source = gameObjectReference };
                _poolKeyCache[gameObjectReference] = key;
                _semaphore.Release();
                
                return await Rent(key);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error renting from reference {gameObjectReference.AssetGUID}: {ex.Message}");
                _semaphore?.Release();
                throw;
            }
        }

        public async UniTask<GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference)
        {
            ThrowIfDisposed();
    
            AssetRefGameObjectItemPool pool = null;
            try
            {
                await _semaphore.WaitAsync();
                if (!_pools.TryGetValue(gameObjectReference, out pool))
                {
                    pool = new AssetRefGameObjectItemPool(gameObjectReference);
                    pool.OnItemDestroyAction += RemoveTrackingItem;
                    pool.OnReturnAction += RemoveTrackingItem;
                    _pools.Add(gameObjectReference, pool);
                }
                _semaphore.Release();

                // Sửa phần gây deadlock
                var item = await pool.Rent();
                int retryCount = 0;
                const int maxRetries = 5; // Giới hạn số lần thử
        
                while (!item && retryCount < maxRetries)
                {
                    retryCount++;
                    await UniTask.NextFrame();
                    item = await pool.Rent();
                }
        
                if (!item)
                {
                    Debug.LogError($"Failed to rent item after {maxRetries} attempts from pool {gameObjectReference.Source.RuntimeKey}");
                    return null;
                }
        
                await _semaphore.WaitAsync();
                var keyInstance = item.GetInstanceID();
                if (_dicTrackingInstancePools.ContainsKey(keyInstance))
                    Debug.LogError($"Duplicate key pool {gameObjectReference.Source.RuntimeKey} for instance {keyInstance}");
                if(keyInstance == 0)
                    Debug.LogError($"Invalid Instance {gameObjectReference.Source.RuntimeKey}");
                _dicTrackingInstancePools[keyInstance] = pool;
                _semaphore.Release();
                return item;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error renting from pool {gameObjectReference.Source.RuntimeKey}: {ex.Message}");
                _semaphore?.Release();
                throw;
            }
        }


        public void Return(GameObject gameObject)
        {
            if (!gameObject || _isDisposed)
                return;

            try
            {
                _semaphore.Wait();
                if (_dicTrackingInstancePools.TryGetValue(gameObject.GetInstanceID(), out var pool))
                {
                    _semaphore.Release();
                    pool.Return(gameObject);
                }
                else
                {
                    _semaphore.Release();
                    Debug.LogWarning($"GameObject {gameObject.name} is not registered in the pool or was already returned.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error returning GameObject {gameObject.name}: {ex.Message}");
                _semaphore?.Release();
            }
        }

        public void Return(AssetRefGameObjectPrefab gameObjectReference, GameObject gameObject)
        {
            if (_isDisposed)
                return;

            try
            {
                _semaphore.Wait();
                if (_pools.TryGetValue(gameObjectReference, out var pool))
                {
                    _semaphore.Release();
                    pool.Return(gameObject);
                }
                else
                {
                    _semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error returning GameObject with reference: {ex.Message}");
                _semaphore?.Release();
            }
        }

        public void ReleaseInstances(int keep, System.Action<GameObject> onReleased = null)
        {
            if (_isDisposed)
                return;

            try
            {
                _semaphore.Wait();
                foreach (var pool in _pools.Values)
                {
                    pool.ReleaseInstances(keep, onReleased);
                }
                _semaphore.Release();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error releasing instances: {ex.Message}");
                _semaphore?.Release();
            }
        }

        private void RemoveTrackingItem(GameObject gameObject)
        {
            if (_isDisposed)
                return;

            try
            {
                _semaphore.Wait();
                _dicTrackingInstancePools.Remove(gameObject.GetInstanceID());
                _semaphore.Release();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error removing tracking item: {ex.Message}");
                _semaphore?.Release();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(GlobalAssetRefGameObjectPool));
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            try
            {
                foreach (var pool in _pools.Values)
                {
                    pool.Dispose();
                }
                
                _pools.Clear();
                _dicTrackingInstancePools.Clear();
                _poolKeyCache.Clear();
                _poolStringKeyCache.Clear();

                if (_semaphore != null)
                {
                    _semaphore.Dispose();
                    _semaphore = null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error during disposal: {ex.Message}");
            }
        }
        
        private class AssetRefGameObjectPrefabEqualityComparer : IEqualityComparer<AssetRefGameObjectPrefab>
        {
            public bool Equals([NotNull] AssetRefGameObjectPrefab x, [NotNull] AssetRefGameObjectPrefab y)
                => y is { Source: not null } && x is { Source: not null } &&
                   x.Source.AssetGUID.Equals(y.Source.AssetGUID);
                   
            public int GetHashCode(AssetRefGameObjectPrefab obj) 
                => obj.Source.AssetGUID.GetHashCode();
        }
    }
}