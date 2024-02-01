using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;
using Object = UnityEngine.Object;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public class PoolItem<T, TPrefab> : MonoBehaviour where T : Object where TPrefab : IPrefab<T>
    {
        private float _lifeTime;
        private T _instance;
        private TPrefab _prefab;
        private UnityPool<T, TPrefab> _pool;

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
            await UniTask.Delay(TimeSpan.FromSeconds(_lifeTime), cancellationToken: this.GetCancellationTokenOnDestroy());
            _pool?.Return(_instance);
        }

        public void SetUp(UnityPool<T, TPrefab> pool, T instance, TPrefab prefab)
        {
            _instance = instance;
            _pool = pool;
            _prefab = prefab;
        }

        private void OnDestroy()
        {
            _pool?.RemoveItem(_instance);
            _prefab?.Release(_instance);
        }
    }
}