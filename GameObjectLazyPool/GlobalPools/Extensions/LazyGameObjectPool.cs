using Cysharp.Threading.Tasks;
using ZBase.Foundation.Pooling.UnityPools;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool.Extensions
{
    
    public static class LazyGameObjectPool
    {
        private static GlobalGameObjectPool GlobalGameObjectPool => SharedPool.Of<GlobalGameObjectPool>();

        public static async UniTask<UnityEngine.GameObject> Rent(UnityEngine.GameObject gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);
        
        public static async UniTask<UnityEngine.GameObject> Rent(GameObjectPrefab gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);
        
        public static void Return(UnityEngine.GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObject);
        
        public static void Return(GameObjectPrefab gameObjectReference, UnityEngine.GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObjectReference, gameObject);
        
        public static void ReleaseInstances(int keep, System.Action<UnityEngine.GameObject> onReleased = null)
            => GlobalGameObjectPool.ReleaseInstances(keep, onReleased);
    }
}