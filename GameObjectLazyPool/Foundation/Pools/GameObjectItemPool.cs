using System;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool
{
    public sealed class GameObjectItemPool : GameObjectPool
    {
        internal event Action<GameObject> OnReturn;
        internal event Action<GameObjectItemPool> OnPoolEmpty;
        private readonly List<int > _poolItems = new();

        internal int ID { get; }

        public GameObjectItemPool(GameObjectPrefab prefab) : base(prefab) => ID = Prefab.Source.GetInstanceID();
        
        protected override void ProcessNewInstance(GameObject instance)
        {
            base.ProcessNewInstance(instance);
            if (!instance.TryGetComponent<GameObjectPoolItem>(out var poolItem))
                poolItem = instance.AddComponent<GameObjectPoolItem>();
            poolItem.SetUp(this, Prefab);
            poolItem.OnItemDestroy += OnItemDestroy;
            this._poolItems.Add(instance.GetInstanceID());
        }

        private void OnItemDestroy(GameObject instance)
        {
            this._poolItems.Remove(instance.GetInstanceID());
            if(this._poolItems.Count == 0)
                OnPoolEmpty?.Invoke(this);
        }
        
        protected override void ReturnPreprocess(GameObject instance)
        {
            base.ReturnPreprocess(instance);
            OnReturn?.Invoke(instance);
        }
    }
}