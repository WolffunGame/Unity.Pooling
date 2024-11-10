using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZBase.Foundation.Pooling;

namespace Unity.Pooling.Scriptables.AddressableAssets
{
    [CreateAssetMenu(
        fileName = "Scriptable AssetRef GameObject Source"
        , menuName = "Pooling/Scriptables/Sources/AssetRef GameObject"
        , order = 1
    )]
    public class ScriptableAssetRefGameObjectSource
        : ScriptableAssetRefSource<AssetReferenceGameObject>
    {
        public override async UniTask<Object> InstantiateAsync(Transform parent,
            CancellationToken cancelToken = default)
        {
            var source = Source;
            if (source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            AsyncOperationHandle<GameObject> handle = default;
            if (source != null)
                handle = parent ? source.InstantiateAsync(parent, true) : source.InstantiateAsync();
            return await handle.WithCancellation(cancelToken);
        }

        public override Object Instantiate(Transform parent)
        {
            var source = Source;

            if (source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            return parent
                ? source?.InstantiateAsync(parent, true).WaitForCompletion()
                : source?.InstantiateAsync().WaitForCompletion();
        }

        public override void Release(Object instance)
        {
            if (instance is GameObject gameObject && Source != null)
                Source.ReleaseInstance(gameObject);
        }
    }
}