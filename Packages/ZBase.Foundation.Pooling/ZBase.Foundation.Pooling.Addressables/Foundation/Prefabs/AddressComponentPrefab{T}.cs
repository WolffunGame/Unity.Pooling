using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ZBase.Foundation.Pooling.AddressableAssets
{
    [Serializable]
    public class AddressComponentPrefab<T> : AddressPrefab<T> where T : Component
    {
        protected override async UniTask<T> InstantiateAsync(
            string source
            , Transform parent
            , CancellationToken cancelToken = default
        )
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? Addressables.InstantiateAsync(source, parent) : Addressables.InstantiateAsync(source);
            var gameObject = await handle.WithCancellation(cancelToken);
            return gameObject.GetComponent<T>();
        }

        protected override T Instantiate(string source, Transform parent, CancellationToken cancelToken = default)
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? Addressables.InstantiateAsync(source, parent) : Addressables.InstantiateAsync(source);
            var gameObject = handle.WaitForCompletion();
            return gameObject.GetComponent<T>();
        }

        public override void Release(T instance)
        {
            if (instance)
                Addressables.ReleaseInstance(instance.gameObject);
        }

       
    }
}