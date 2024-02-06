using System;
using UnityEngine;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool
{
    public class AssetRefGameObjectItemPool : AssetRefGameObjectPool
    {
        internal event Action<GameObject> OnReturn;
        
        public AssetRefGameObjectItemPool(AssetRefGameObjectPrefab prefab) : base(prefab)
        {
        }
        
        protected override void ProcessNewInstance(GameObject instance)
        {
            base.ProcessNewInstance(instance);
            if (!instance.TryGetComponent<AssetRefGameObjectPoolItem>(out var poolItem))
                poolItem = instance.AddComponent<AssetRefGameObjectPoolItem>();
            poolItem.SetUp(this);
        }

        protected override void ReturnPreprocess(GameObject instance)
        {
            base.ReturnPreprocess(instance);
            OnReturn?.Invoke(instance);
        }
    }
}