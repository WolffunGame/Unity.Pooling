using System.Threading;
using Cysharp.Threading.Tasks;

namespace ZBase.Foundation.Pooling
{
    public interface IAsyncInstantiable<T> : IInstantiable<UniTask<T>>
    {
        UniTask<T> InstantiateAsync(CancellationToken cancelToken);
    }
}
