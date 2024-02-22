using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZBase.Foundation.Pooling.ScriptablePools
{
    public class ScriptableComponentSource<T> : ScriptableSource<T> where T : Component
    {
        public override async UniTask<Object> InstantiateAsync(Transform parent, CancellationToken cancelToken = default)
        {
            var source = Source;
            if (source == false)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            T instance = parent ? Instantiate(source, parent) : Instantiate(source);
            return await UniTask.FromResult(instance);
        }

        public override Object Instantiate(Transform parent)
        {
            var source = Source;
            if (source == false)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.source);
            return Instantiate(source, parent);
        }

        public override void Release(Object instance)
        {
            if (instance is T component)
                Destroy(component.gameObject);
        }
    }
}
