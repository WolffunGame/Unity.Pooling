using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;
using Object = UnityEngine.Object;

namespace ZBase.Foundation.Pooling.AddressableAssets.Items
{
    public class PoolItem<T, TPrefab> : MonoBehaviour where T : Object where TPrefab : IPrefab<T>
    {
        private T _instance;
        private TPrefab _prefab;
        private UnityPool<T, TPrefab> _pool;

        public void SetUp(UnityPool<T, TPrefab> pool, T instance, TPrefab prefab)
        {
            _instance = instance;
            _pool = pool;
            _prefab = prefab;
        }

        private void OnDestroy()
        {
            _pool.RemoveItem(_instance);
            _prefab.Release(_instance);
        } 
    }
}