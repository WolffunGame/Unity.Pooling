using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ZBase.Foundation.Pooling.UnityPools
{
    [Serializable]
    public class GameObjectPool<TPrefab> : UnityPool<GameObject, TPrefab> where TPrefab : IPrefab<GameObject>
    {
        [SerializeField] private bool _dontApplyPrefabParentOnReturn;
        public event Action<GameObject> OnReturnAction;

        public GameObjectPool()
        {
        }

        public GameObjectPool(TPrefab prefab) : base(prefab)
        {
        }

        public GameObjectPool(UniqueQueue<int, GameObject> queue) : base(queue)
        {
        }

        public GameObjectPool(UniqueQueue<int, GameObject> queue, TPrefab prefab) : base(queue, prefab)
        {
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void ReturnPreprocess(GameObject instance)
        {
            if (!instance) 
                return;
            instance.SetActive(false);
            if (_dontApplyPrefabParentOnReturn == false && Prefab != null)
                instance.transform.SetParent(Prefab.Parent);
            OnReturnAction?.Invoke(instance);
        }
    }
}