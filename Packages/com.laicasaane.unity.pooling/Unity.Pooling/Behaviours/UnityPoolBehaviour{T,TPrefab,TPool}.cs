﻿using System.Pooling.Statistics;
using System.Threading;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Unity.Pooling
{
    public abstract class UnityPoolBehaviour<T, TPrefab, TPool>
        : PoolBehaviour<T, TPool>, IPrepoolable
        where T : UnityEngine.Object
        where TPrefab : IPrefab<T>
        where TPool : IUnityPool<T, TPrefab>
    {
        [SerializeField]
        private bool _prepoolOnStart = false;

        public bool PrepoolOnStart
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _prepoolOnStart;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _prepoolOnStart = value;
        }

        private readonly UnityPrepooler<T, TPrefab, TPool> _prepooler = default;

        protected void Awake()
        {
            if (Pool == null || Pool.Prefab == null)
                return;

            if (Pool.Prefab.Parent == false)
                Pool.Prefab.Parent = this.transform;

            OnAwake();
        }

        protected async UniTask Start()
        {
            if (_prepoolOnStart)
                await Prepool(this.GetCancellationTokenOnDestroy());

            await OnStart();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask Prepool(CancellationToken cancelToken)
        {
            await _prepooler.Prepool(Pool.Prefab, Pool, this.transform, cancelToken);
            PoolTracker.TrackPoolRentOrCreate(this, Pool.Prefab.PrepoolAmount);
        }
        
        protected virtual void OnAwake() { }

        protected async UniTask OnStart()
            => await UniTask.Delay(0);
    }
}
