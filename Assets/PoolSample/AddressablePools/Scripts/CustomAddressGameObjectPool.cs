using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling;
using ZBase.Foundation.Pooling.AddressableAssets;
using ZBase.Foundation.Pooling.UnityPools;

namespace Pooling.Sample
{
    public class CustomAddressGameObjectPool : AddressGameObjectPool
    {
        private readonly UnityPrePool<GameObject, AddressGameObjectPrefab, AddressGameObjectPool> _prePool;

        public CustomAddressGameObjectPool()
            : base()
        {
            this._prePool = new UnityPrePool<GameObject, AddressGameObjectPrefab, AddressGameObjectPool>();
        }

        public CustomAddressGameObjectPool(AddressGameObjectPrefab prefab)
            : base(prefab)
        {
            this._prePool = new UnityPrePool<GameObject, AddressGameObjectPrefab, AddressGameObjectPool>();
        }

        public CustomAddressGameObjectPool(UniqueQueue<int, GameObject> queue)
            : base(queue)
        {
            this._prePool = new UnityPrePool<GameObject, AddressGameObjectPrefab, AddressGameObjectPool>();
        }

        public CustomAddressGameObjectPool(UniqueQueue<int, GameObject> queue, AddressGameObjectPrefab prefab)
            : base(queue, prefab)
        {
            this._prePool = new UnityPrePool<GameObject, AddressGameObjectPrefab, AddressGameObjectPool>();
        }

        public async UniTask Prepool()
        {
            await this._prePool.PrePool(Prefab, this, Prefab.Parent);
        }
    }
}