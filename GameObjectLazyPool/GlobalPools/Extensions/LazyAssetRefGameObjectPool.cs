using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool.Extensions
{
    
    public static class LazyAssetRefGameObjectPool
    {
        private static GlobalAssetRefGameObjectPool GlobalGameObjectPool => SharedPool.Of<GlobalAssetRefGameObjectPool>();

        public static async UniTask<UnityEngine.GameObject> Rent(AssetReferenceGameObject gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);
        
        public static async UniTask<UnityEngine.GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);
        
        public static void Return(UnityEngine.GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObject);
        
        public static void Return(AssetRefGameObjectPrefab gameObjectReference, UnityEngine.GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObjectReference, gameObject);
        
        public static void ReleaseInstances(int keep, System.Action<UnityEngine.GameObject> onReleased = null)
            => GlobalGameObjectPool.ReleaseInstances(keep, onReleased);
    }
}