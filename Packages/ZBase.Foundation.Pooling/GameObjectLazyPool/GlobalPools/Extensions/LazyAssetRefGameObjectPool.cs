using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool.Extensions
{
    
    public static class LazyAssetRefGameObjectPool
    {
        private static GlobalAssetRefGameObjectPool GlobalGameObjectPool => SharedPool.Of<GlobalAssetRefGameObjectPool>();

        public static async UniTask<GameObject> Rent(string address)
            => await GlobalGameObjectPool.Rent(address);

        public static async UniTask<GameObject> Rent(AssetReferenceGameObject gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);

        public static async UniTask<GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);
        
        public static void Return(GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObject);
        
        public static void Return(AssetRefGameObjectPrefab gameObjectReference, GameObject gameObject)
            => GlobalGameObjectPool.Return(gameObjectReference, gameObject);
        
        public static void ReleaseInstances(int keep, System.Action<GameObject> onReleased = null)
            => GlobalGameObjectPool.ReleaseInstances(keep, onReleased);
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Dispose() => GlobalGameObjectPool.Dispose();
    }
}