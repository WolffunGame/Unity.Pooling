using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace Pooling.Sample
{
    /// <summary>
    /// A concrete implementation of a GameObjectPool. In this example we are using a UnityPrepooler to prepool the GameObjects.
    /// </summary>
    public class CustomGameObjectPool : GameObjectPool
    {
        private readonly UnityPrePool<GameObject, GameObjectPrefab, GameObjectPool> _prePool = new();

        public void PrePool() => this._prePool.PrePool(Prefab, this, Prefab.Parent).Forget();
    }
}