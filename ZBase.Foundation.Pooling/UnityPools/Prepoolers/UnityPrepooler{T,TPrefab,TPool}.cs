using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZBase.Foundation.Pooling.UnityPools
{
    public struct UnityPrePool<T, TPrefab, TPool>
        : IPrePool<T, TPrefab, TPool> where T : Object where TPrefab : IPrefab<T> where TPool : IReturnable<T>
    {
        public readonly async UniTask PrePool(
            TPrefab prefab , TPool pool , Transform defaultParent , CancellationToken cancelToken = default )
        {
            if (prefab == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.prefab);
                return;
            }

            if (pool == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.pool);
                return;
            }

            if (prefab.PrePoolAmount <= 0)
                return;

            if (prefab.Parent == false && defaultParent)
                prefab.Parent = defaultParent;

            for (int i = 0; i < prefab.PrePoolAmount; i++)
            {
                var instance = await prefab.Instantiate(cancelToken);
                pool.Return(instance);
            }
        }
    }
}