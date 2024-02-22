using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZBase.Foundation.Pooling.ScriptablePools
{
    [CreateAssetMenu(
        fileName = "Scriptable GameObject Source"
        , menuName = "Pooling/Scriptables/Sources/GameObject"
        , order = 1
    )]
    public class ScriptableGameObjectSource : ScriptableSource<GameObject>
    {
        public override async UniTask<Object> InstantiateAsync(Transform parent, CancellationToken cancelToken = default)
        {
            var source = Source;

            if (source == false)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            GameObject instance = parent ? Instantiate(source, parent) : Instantiate(source);
            return await UniTask.FromResult(instance);
        }

        public override Object Instantiate(Transform parent)
        {
            var source = Source;
            if (source == false)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            return parent
                ? Instantiate(source, parent)
                : Instantiate(source);
        }

        public override void Release(Object instance)
        {
            if (instance is GameObject gameObject)
                Destroy(gameObject);
        }
    }
}
