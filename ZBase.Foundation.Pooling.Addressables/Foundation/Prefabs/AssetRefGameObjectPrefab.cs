using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ZBase.Foundation.Pooling.AddressableAssets
{
    [Serializable]
    public class AssetRefGameObjectPrefab : AssetRefPrefab<GameObject, AssetReferenceGameObject>,
        IEquatable<AssetRefGameObjectPrefab>, IEqualityComparer<AssetRefGameObjectPrefab>
    {
        protected override async UniTask<GameObject> Instantiate(
            AssetReferenceGameObject source, Transform parent, CancellationToken cancelToken = default)
        {
            AsyncOperationHandle<GameObject> handle =
                parent ? source.InstantiateAsync(parent, true) : source.InstantiateAsync();
            return await handle.WithCancellation(cancelToken);
        }

        public override void Release(GameObject instance)
        {
            if (instance && Source != null)
                Source.ReleaseInstance(instance);
        }

        public bool Equals(AssetRefGameObjectPrefab other) => other != null && this.Source.Equals(other.Source);

        public bool Equals(AssetRefGameObjectPrefab x, AssetRefGameObjectPrefab y)
            => x != null && y != null && x.Source.Equals(y.Source);

        public int GetHashCode(AssetRefGameObjectPrefab obj) => obj.Source.GetHashCode();
    }
}