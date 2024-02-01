using System;
using UnityEngine;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public class AssetRefGameObjectItemPool : AssetRefGameObjectPool
    {
        internal event Action<UnityEngine.GameObject> OnReturn;
        
        public AssetRefGameObjectItemPool(AssetRefGameObjectPrefab prefab) : base(prefab)
        {
        }
        
        protected override void ProcessNewInstance(UnityEngine.GameObject instance)
        {
            base.ProcessNewInstance(instance);
            if (!instance.TryGetComponent<AssetRefGameObjectPoolItem>(out var poolItem))
                poolItem = instance.AddComponent<AssetRefGameObjectPoolItem>();
            poolItem.SetUp(this, instance, Prefab);
        }

        protected override void ReturnPreprocess(UnityEngine.GameObject instance)
        {
            base.ReturnPreprocess(instance);
            OnReturn?.Invoke(instance);
        }
    }
}