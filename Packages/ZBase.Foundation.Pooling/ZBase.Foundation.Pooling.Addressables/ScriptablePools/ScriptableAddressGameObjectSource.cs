using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using ZBase.Foundation.Pooling;

namespace Unity.Pooling.Scriptables.AddressableAssets
{
    [CreateAssetMenu(
        fileName = "Scriptable Asset Name GameObject Source"
        , menuName = "Pooling/Scriptables/Sources/Asset Name GameObject"
        , order = 1
    )]
    public class ScriptableAddressGameObjectSource : ScriptableAddressSource
    {
        public override async UniTask<Object> InstantiateAsync(Transform parent,
            CancellationToken cancelToken = default)
        {
            var source = Source;

            if (string.IsNullOrEmpty(source))
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.assetName);
            AsyncOperationHandle<GameObject> handle = parent
                ? Addressables.InstantiateAsync(source, parent, true)
                : Addressables.InstantiateAsync(source);
            return await handle.WithCancellation(cancelToken);
        }

        public override Object Instantiate(Transform parent)
        {
            var source = Source;
            if (string.IsNullOrEmpty(source))
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.assetName);
            return parent
                ? Addressables.InstantiateAsync(source, parent, true).WaitForCompletion()
                : Addressables.InstantiateAsync(source).WaitForCompletion();
        }

        public override void Release(Object instance)
        {
            if (instance is GameObject gameObject)
                Addressables.ReleaseInstance(gameObject);
        }
    }
}