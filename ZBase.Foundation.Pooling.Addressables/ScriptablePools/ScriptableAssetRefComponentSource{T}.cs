using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZBase.Foundation.Pooling;

namespace Unity.Pooling.Scriptables.AddressableAssets
{
    public class ScriptableAssetRefComponentSource<T>
        : ScriptableAssetRefSource<AssetReferenceGameObject>
        where T : Component
    {
        public override async UniTask<Object> InstantiateAsync(Transform parent, CancellationToken cancelToken = default)
        {
            var source = Source;

            if (source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            AsyncOperationHandle<GameObject> handle = default;
            if (source != null)
                handle = parent ? source.InstantiateAsync(parent) : source.InstantiateAsync();
            var gameObject = await handle.WithCancellation(cancelToken);
            return gameObject.GetComponent<T>();
        }

        public override Object Instantiate(Transform parent)
        {
            var source = Source;
            if (source == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            var gameObject = parent
                ? source?.InstantiateAsync(parent).WaitForCompletion()
                : source?.InstantiateAsync().WaitForCompletion();
            return gameObject != null ? gameObject.GetComponent<T>() : null;
        }

        public override void Release(Object instance)
        {
            if (instance is T component && Source != null)
                Source.ReleaseInstance(component.gameObject);
        }
    }
}
