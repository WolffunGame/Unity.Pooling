using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZBase.Foundation.Pooling.AddressableAssets;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool.Extensions
{
    
    public static class LazyAssetRefGameObjectPool
    {
        private static GlobalAssetRefGameObjectPool GlobalGameObjectPool => SharedPool.Of<GlobalAssetRefGameObjectPool>();

        #region Preload

        public static async UniTask Preload(string address) =>
            GlobalGameObjectPool.Return(await GlobalGameObjectPool.Rent(address));
        
        public static async UniTask Preload(AssetReferenceGameObject gameObjectReference) =>
            GlobalGameObjectPool.Return(await GlobalGameObjectPool.Rent(gameObjectReference));
        
        public static async UniTask Preload(AssetRefGameObjectPrefab gameObjectReference) =>
            GlobalGameObjectPool.Return(await GlobalGameObjectPool.Rent(gameObjectReference));

        #endregion

        public static async UniTask<GameObject> Rent(string address)
            => await GlobalGameObjectPool.Rent(address);

        public static async UniTask<GameObject> Rent(AssetReferenceGameObject gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);

        public static async UniTask<GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference)
            => await GlobalGameObjectPool.Rent(gameObjectReference);

        public static async UniTask<GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference, Vector3 pos, bool activeOnSpawn = true)
        {
            var go = await GlobalGameObjectPool.Rent(gameObjectReference);
            go.transform.position = pos;
            go.SetActive(activeOnSpawn);
            return go;
        }
        
        public static async UniTask<GameObject> Rent(AssetRefGameObjectPrefab gameObjectReference, Vector3 pos, Quaternion rot, bool activeOnSpawn = true)
        {
            var go = await GlobalGameObjectPool.Rent(gameObjectReference);
            go.transform.SetPositionAndRotation(pos, rot);
            go.SetActive(activeOnSpawn);
            return go;
        }
        
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