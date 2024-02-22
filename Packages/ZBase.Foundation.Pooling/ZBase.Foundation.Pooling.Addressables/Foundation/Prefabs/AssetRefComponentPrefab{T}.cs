using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ZBase.Foundation.Pooling.AddressableAssets
{
    [Serializable]
    public class AssetRefComponentPrefab<T> : AssetRefPrefab<T, AssetReferenceGameObject>
        where T : Component
    {
        protected override async UniTask<T> InstantiateAsync(AssetReferenceGameObject source, Transform parent,
            CancellationToken cancelToken = default)
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? source.InstantiateAsync(parent, true) : source.InstantiateAsync();
            var gameObject = await handle.WithCancellation(cancelToken);
            return gameObject.GetComponent<T>();
        }

        protected override T Instantiate(AssetReferenceGameObject source, Transform parent, CancellationToken cancelToken = default)
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? source.InstantiateAsync(parent, true) : source.InstantiateAsync();
            var gameObject = handle.WaitForCompletion();
            return gameObject.GetComponent<T>();
        }

        public override void Release(T instance)
        {
            if (instance && Source != null)
                Source.ReleaseInstance(instance.gameObject);
        }
    }
}