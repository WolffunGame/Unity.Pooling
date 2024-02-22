using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool.Extensions
{
    
    public static class LazyGameObjectPool
    {
        private static GlobalGameObjectPool GlobalGameObjectPool => SharedPool.Of<GlobalGameObjectPool>();
        
        public static  GameObject RentSync(GameObject gameObjectReference)
            =>  GlobalGameObjectPool.RentSync(gameObjectReference);
        
        public static async UniTask<GameObject> Rent(GameObject gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);
        public static async UniTask<GameObject> Rent(GameObjectPrefab gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);
        public static void Return(GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObject);
        
        public static void Return(GameObjectPrefab gameObjectReference, GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObjectReference, gameObject);
        
        public static void ReleaseInstances(int keep, System.Action<GameObject> onReleased = null)
            => GlobalGameObjectPool.ReleaseInstances(keep, onReleased);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Dispose() => GlobalGameObjectPool.Dispose();
    }
}