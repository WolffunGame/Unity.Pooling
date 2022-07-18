﻿using System;
using System.Pooling;
using Collections.Pooled.Generic;
using Collections.Pooled.Generic.Internals.Unsafe;
using Cysharp.Threading.Tasks;

namespace Unity.Pooling
{
    public abstract partial class AsyncUnityPoolBase<TKey, T> : IAsyncUnityPool<TKey, T>, IAsyncInstantiatorSetable<T>, IDisposable
        where T : UnityEngine.Object
    {
        private readonly Dictionary<TKey, Queue<T>> _queueMap;
        private UniTaskFunc<T> _instantiate;

        public AsyncUnityPoolBase()
            : this(null)
        { }

        public AsyncUnityPoolBase(UniTaskFunc<T> instantiate)
        {
            _queueMap = new Dictionary<TKey, Queue<T>>();
            _instantiate = instantiate ?? GetDefaultInstantiator() ?? DefaultAsyncInstantiator<T>.Get();
        }

        public void SetInstantiator(UniTaskFunc<T> instantiator)
            => _instantiate = instantiator ?? GetDefaultInstantiator();

        public int Count(TKey key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (_queueMap.TryGetValue(key, out var queue))
                return queue.Count;

            return 0;
        }

        public void Dispose()
        {
            _queueMap.GetUnsafe(out var entries, out var count);

            for (int i = 0; i < count; i++)
            {
                ref var entry = ref entries[i];

                if (entry.Next >= -1)
                {
                    entry.Value?.Dispose();
                }
            }

            _queueMap.Dispose();
        }

        /// <inheritdoc/>
        public void ReleaseInstances(TKey key, int keep, Action<T> onReleased = null)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (_queueMap.TryGetValue(key, out var queue))
            {
                var countRemove = queue.Count - keep;

                while (countRemove > 0)
                {
                    var instance = queue.Dequeue();
                    onReleased?.Invoke(instance);
                    countRemove--;
                }
            }
        }

        public async UniTask<T> RentAsync(TKey key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (_queueMap.TryGetValue(key, out var queue))
            {
                if (queue.Count > 0)
                    return queue.Dequeue();
            }

            return await _instantiate();
        }

        public async UniTask<T> RentAsync(TKey key, string name)
        {
            var instance = await RentAsync(key);
            instance.name = name.NameOfIfNullOrEmpty<T>();
            return instance;
        }

        public void Return(TKey key, T instance)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (instance == false)
                return;

            if (_queueMap.TryGetValue(key, out var queue) == false)
            {
                queue = new Queue<T>();
                _queueMap[key] = queue;
            }

            ReturnPreprocess(instance);
            queue.Enqueue(instance);
        }

        protected abstract void ReturnPreprocess(T instance);

        protected abstract UniTaskFunc<T> GetDefaultInstantiator();
    }
}
