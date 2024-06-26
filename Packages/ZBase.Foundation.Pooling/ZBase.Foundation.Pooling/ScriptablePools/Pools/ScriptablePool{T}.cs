using System;
using System.Threading;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.ScriptablePools
{
    public class ScriptablePool<T>
        : ScriptableObject, IUnityPool<T, UnityObjectPrefab>, IPrepoolable, IHasParent
        where T : UnityEngine.Object
    {
        [SerializeField]
        private UnityObjectPrefab _prefab;

        [SerializeField]
        private bool _prepoolOnStart = false;

        private readonly UnityObjectPool _pool = new();
        private readonly UnityObjectPrePool _prePool = default;

        public bool PrepoolOnStart
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _prepoolOnStart;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _prepoolOnStart = value;
        }

        public UnityObjectPrefab Prefab
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _prefab;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _prefab = value;
        }

        public Transform Parent
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _prefab.Parent;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _prefab.Parent = value ?? throw new ArgumentNullException(nameof(value));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count()
            => _pool.Count();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(T instance)
            => _pool.Return(instance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask Prepool(CancellationToken cancelToken)
            => await this._prePool.PrePool(_prefab, _pool, Parent, cancelToken);

        public void ReleaseInstances(int keep, Action<T> onReleased = null)
        {
            void OnRelease(UnityEngine.Object instance)
            {
                if (onReleased != null && instance is T instanceT)
                {
                    onReleased(instanceT);
                    return;
                }

                if (_prefab != null)
                    _prefab.Release(instance);
            }

            _pool.ReleaseInstances(keep, OnRelease);
        }

        public async UniTask<T> Rent()
        {
            _pool.Prefab = _prefab;

            var instance = await _pool.Rent();

            if (instance is T instanceT)
                return instanceT;

            if (instance is GameObject gameObject)
            {
                if (gameObject.TryGetComponent<T>(out var component))
                    return component;
            }

            throw new InvalidCastException($"Cannot cast {instance.GetType()} into {typeof(T)}");
        }

        public async UniTask<T> Rent(CancellationToken cancelToken)
        {
            _pool.Prefab = _prefab;

            var instance = await _pool.Rent(cancelToken);

            if (instance is T instanceT)
                return instanceT;

            if (instance is GameObject gameObject)
            {
                if (gameObject.TryGetComponent<T>(out var component))
                    return component;
            }

            throw new InvalidCastException($"Cannot cast {instance.GetType()} into {typeof(T)}");
        }

        protected virtual void OnDestroy()
        {
            _pool.Dispose();
        }
    }
}
