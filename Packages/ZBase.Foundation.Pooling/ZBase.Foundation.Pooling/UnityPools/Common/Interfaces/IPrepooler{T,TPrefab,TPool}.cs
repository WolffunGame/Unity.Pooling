using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZBase.Foundation.Pooling.UnityPools
{
    public interface IPrePool<T, in TPrefab, in TPool> where TPrefab : IPrefab<T> where TPool : IReturnable<T>
    {
        UniTask PrePool(
              TPrefab prefab
            , TPool pool
            , Transform defaultParent
            , CancellationToken cancelToken = default
        );
    }
}