﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ZBase.Foundation.Pooling.AddressableAssets
{
    [Serializable]
    public class AddressGameObjectPrefab : AddressPrefab<GameObject>
    {
        protected override async UniTask<GameObject> Instantiate(
            string source, Transform parent, CancellationToken cancelToken = default)
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? Addressables.InstantiateAsync(source, parent) : Addressables.InstantiateAsync(source);
            return await handle.WithCancellation(cancelToken);
        }

        public override void Release(GameObject instance)
        {
            if (instance)
                Addressables.ReleaseInstance(instance);
        }
    }
}