﻿using System;
using UnityEngine;
using ZBase.Foundation.Pooling;

namespace ZBase.Foundation.Pooling.AddressableAssets
{
    [Serializable]
    public class AddressGameObjectPool : AddressGameObjectPool<AddressGameObjectPrefab>
    {
        public AddressGameObjectPool()
        {
        }

        public AddressGameObjectPool(AddressGameObjectPrefab prefab)
            : base(prefab)
        {
        }

        public AddressGameObjectPool(UniqueQueue<int, GameObject> queue) : base(queue)
        {
        }

        public AddressGameObjectPool(UniqueQueue<int, GameObject> queue, AddressGameObjectPrefab prefab) : base(queue,
            prefab)
        {
        }
    }
}