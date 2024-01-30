using System.Runtime.CompilerServices;
using UnityEngine;

namespace ZBase.Foundation.Pooling
{
    public static class SharedPool
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Of<T>() where T : IPool, IShareable, new() => SharedInstance<T>.Instance;

        private static class SharedInstance<T> where T : IPool, IShareable, new()
        {
            public static T Instance { get; private set; }
            static SharedInstance() => Init();
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            static void Init() => Instance = new T();
        }
    }
}