﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ZBase.Foundation.Pooling.AddressableAssets
{
    [Serializable]
    public class AssetRefGameObjectPrefab : AssetRefPrefab<GameObject, AssetReferenceGameObject>
    {
        protected override async UniTask<GameObject> InstantiateAsync(
            AssetReferenceGameObject source, Transform parent, CancellationToken cancelToken = default)
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? source.InstantiateAsync(parent, true) : source.InstantiateAsync();
            return await handle.WithCancellation(cancelToken);
        }

        protected override GameObject Instantiate(AssetReferenceGameObject source, Transform parent, CancellationToken cancelToken = default)
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? source.InstantiateAsync(parent, true) : source.InstantiateAsync();
            return handle.WaitForCompletion();
        }

        public override void Release(GameObject instance)
        {
            if (instance && Source != null)
                Source.ReleaseInstance(instance);
        }
    }
}