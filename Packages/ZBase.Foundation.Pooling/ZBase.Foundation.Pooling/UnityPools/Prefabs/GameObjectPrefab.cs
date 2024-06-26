﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ZBase.Foundation.Pooling.UnityPools
{
    [Serializable]
    public class GameObjectPrefab : UnityPrefab<GameObject, GameObject>
    {
        protected override async UniTask<GameObject> InstantiateAsync(
            GameObject source, Transform parent, CancellationToken cancelToken = default)
        {
            GameObject instance = parent
                ? UnityEngine.Object.Instantiate(this.Source, parent, true)
                : UnityEngine.Object.Instantiate(this.Source);
            return await UniTask.FromResult(instance);
        }

        protected override GameObject Instantiate(GameObject source, Transform parent, CancellationToken cancelToken = default)
        {
            return parent
                ? UnityEngine.Object.Instantiate(this.Source, parent, true)
                : UnityEngine.Object.Instantiate(this.Source);
        }

        public override void Release(GameObject instance)
        {
            if (instance)
                UnityEngine.Object.Destroy(instance);
        }
    }
}