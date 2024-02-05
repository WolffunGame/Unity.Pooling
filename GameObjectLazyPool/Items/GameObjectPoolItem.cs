using System;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;
namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool
{
    public class GameObjectPoolItem : PoolItem<GameObject, GameObjectPrefab>
    {
        public event Action<GameObject> OnItemDestroy;
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.OnItemDestroy?.Invoke(this.gameObject);
        }
    }
}