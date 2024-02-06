using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool
{
    public class PoolItem<TPrefab> : MonoBehaviour where TPrefab : IPrefab<GameObject>
    {
        private float _lifeTime;
        private UnityPool<GameObject, TPrefab> _pool;

        private void Awake()
        {
            var setUp = GetComponent<PoolItemAutoDeSpawnSetUp>();
            if (setUp != null)
                _lifeTime = setUp.LifeTime;
        }

        private void OnEnable()
        {
            if (_lifeTime <= 0)
                return;
            StartDeSpawn().Forget();
        }

        private async UniTask StartDeSpawn()
        {
            if (_lifeTime <= 0)
                return;
            if (await UniTask
                    .Delay(TimeSpan.FromSeconds(_lifeTime), cancellationToken: this.GetCancellationTokenOnDestroy())
                    .SuppressCancellationThrow())
                return;
            _pool?.Return(gameObject);
        }

        public void SetUp(UnityPool<GameObject, TPrefab> pool) => _pool = pool;

        protected virtual void OnDestroy() => _pool?.OnPoolItemDestroy(gameObject);
    }
}