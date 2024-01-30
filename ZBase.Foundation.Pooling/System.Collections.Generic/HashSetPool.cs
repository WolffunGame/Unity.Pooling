using System.Runtime.CompilerServices;
using ZBase.Foundation.Pooling;

namespace System.Collections.Generic.Pooling
{
    public sealed class HashSetPool<T> : Pool<HashSet<T>, DefaultConstructorInstantiator<HashSet<T>>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void ReturnPreprocess(HashSet<T> instance) => instance.Clear();
    }
}