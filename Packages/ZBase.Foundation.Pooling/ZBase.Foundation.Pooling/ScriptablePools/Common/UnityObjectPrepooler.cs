using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.ScriptablePools
{
    public struct UnityObjectPrePool
        : IPrePool<Object
            , UnityObjectPrefab
            , IReturnable<Object>
        >
    {
        public async UniTask PrePool(
              UnityObjectPrefab prefab
            , IReturnable<Object> pool
            , Transform defaultParent
            , CancellationToken cancelToken = default
        )
        {
            if (prefab == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.prefab);

            if (pool == null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.pool);

            if (prefab.PrePoolAmount <= 0)
                return;

            if (prefab.Parent == false && defaultParent)
                prefab.Parent = defaultParent;

            for (int i = 0, count = prefab.PrePoolAmount; i < count; i++)
            {
                var instance = await prefab.Instantiate(cancelToken);
                pool.Return(instance);
            }
        }
    }
}
