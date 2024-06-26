﻿using System;
using System.Runtime.CompilerServices;
using ZBase.Collections.Pooled;
using ZBase.Collections.Pooled.Generic;

namespace ZBase.Foundation.Pooling
{
    public class UniqueQueue<TKey, TValue> : IDisposable
    {
        private readonly Queue<TKey> _queue;
        private readonly Dictionary<TKey, TValue> _unique;

        public UniqueQueue()
        {
            _queue = new Queue<TKey>();
            _unique = new Dictionary<TKey, TValue>();
        }

        public UniqueQueue(Queue<TKey> queue, Dictionary<TKey, TValue> map)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _unique = map ?? throw new ArgumentNullException(nameof(map));
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _queue.Count;
        }

        public void Dispose()
        {
            _unique.Dispose();
            _queue.Dispose();
        }

        public bool TryEnqueue(TKey key, TValue value)
        {
            if (key is null || value is null)
                return false;
            if (!this._unique.TryAdd(key, value))
                return false;
            _queue.Enqueue(key);
            return true;
        }

        public bool TryDequeue(out TValue value)
        {
            value = default;
            while (_queue.Count > 0)
                if (_queue.TryDequeue(out var key) && this._unique.Remove(key, out value) && value != null)
                    return true;
            return false;
        }

        internal void Remove(TKey key) => this._unique.Remove(key, out _);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TKey key)
            => _unique.ContainsKey(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(in KVPair<TKey, TValue> item)
            => _unique.ContainsKey(item.Key);
    }
}