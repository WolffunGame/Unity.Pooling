using System;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public sealed class GameObjectItemPool : GameObjectPool
    {
        internal event Action<UnityEngine.GameObject> OnReturn;
        
        public GameObjectItemPool(GameObjectPrefab prefab) : base(prefab)
        {
        }
        
        protected override void ProcessNewInstance(UnityEngine.GameObject instance)
        {
            base.ProcessNewInstance(instance);
            if (!instance.TryGetComponent<GameObjectPoolItem>(out var poolItem))
                poolItem = instance.AddComponent<GameObjectPoolItem>();
            poolItem.SetUp(this, instance, Prefab);
        }

        protected override void ReturnPreprocess(UnityEngine.GameObject instance)
        {
            base.ReturnPreprocess(instance);
            OnReturn?.Invoke(instance);
        }
    }
}