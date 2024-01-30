using System;
using System.Threading;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace ZBase.Foundation.Pooling.UnityPools
{
    [Serializable]
    public abstract class UnityPrefab<T, TSource>
        : IPrefab<T, TSource>
        where T : class
    {
        [SerializeField] private TSource _source;
        [SerializeField] private Transform _parent;
        [SerializeField] private int _prePoolAmount;

        public TSource Source
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _source;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _source = value;
        }

        public Transform Parent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _parent;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _parent = value;
        }

        public int PrePoolAmount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._prePoolAmount;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this._prePoolAmount = value;
        }

        public async UniTask<T> Instantiate()
        {
            if (_source is null)
                throw new NullReferenceException(nameof(Source));
            return await Instantiate(Source, Parent);
        }

        public async UniTask<T> Instantiate(CancellationToken cancelToken)
        {
            if (_source is null)
                throw new NullReferenceException(nameof(Source));
            return await Instantiate(Source, Parent, cancelToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract UniTask<T> Instantiate(
            TSource source
            , Transform parent
            , CancellationToken cancelToken = default);

        public abstract void Release(T instance);
    }
}