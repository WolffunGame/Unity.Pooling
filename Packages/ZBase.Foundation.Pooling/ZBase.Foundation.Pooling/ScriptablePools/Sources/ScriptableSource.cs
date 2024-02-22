using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.ScriptablePools
{
    public abstract class ScriptableSource : ScriptableObject, IReleasable<Object>
    {
        public abstract UniTask<Object> InstantiateAsync(Transform parent, CancellationToken cancelToken = default);
        public abstract Object Instantiate(Transform parent);

        public abstract void Release(Object instance);
    }
}
