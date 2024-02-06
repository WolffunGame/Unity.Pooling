using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZBase.Collections.Pooled.Generic;
using ZBase.Foundation.Pooling.AddressableAssets;
using ZBase.Foundation.Pooling.GameObjectItem.LazyPool.Extensions;

namespace Pooling.Sample
{
    public class AddressableGameObjectPoolSample1 : MonoBehaviour
    {
        [SerializeField] private AssetRefGameObjectPrefab _prefab;
        [SerializeField] private AssetReferenceGameObject _assetReferenceGameObject;

        private GameObject _item;

        private async UniTask Spawn()
        {
            _item = await LazyAssetRefGameObjectPool.Rent(_prefab);
            _item.SetActive(true);
            var pos = Random.insideUnitCircle * _spawnRadius;
            _item.transform.position = new Vector3 {x = pos.x, y = 0, z = pos.y};
            _spawned.Add(_item);
        } 
        
        private async UniTask SpawnByRef()
        {
            _item = await LazyAssetRefGameObjectPool.Rent(_assetReferenceGameObject);
            _item.SetActive(true);
            var pos = Random.insideUnitCircle * _spawnRadius;
            _item.transform.position = new Vector3 {x = pos.x, y = 0, z = pos.y};
            _spawned.Add(_item);
        } 
        
        private void DeSpawn() => LazyAssetRefGameObjectPool.Return(_item);

        private void Return()
        {
            foreach (var go in _spawned)
            {
                go.SetActive(false);
                LazyAssetRefGameObjectPool.Return(go);
            }
            _spawned.Clear();
        }

        private void ReleaseAll()
        {
            foreach (var go in _spawned)
                go.SetActive(false);
            LazyAssetRefGameObjectPool.ReleaseInstances(0);
            _spawned.Clear();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 50), "Spawn"))
                for (int i = 0; i < _spawnCount; i++)
                    Spawn().Forget();
            if (GUI.Button(new Rect(10, 70, 150, 50), "Return"))
                Return();
            if (GUI.Button(new Rect(10, 130, 150, 50), "Release All"))
                ReleaseAll();
            if (GUI.Button(new Rect(10, 190, 150, 50), "Load By AssetRef"))
                for (int i = 0; i < this._spawnCount; i++)
                    SpawnByRef().Forget();
            if (GUI.Button(new Rect(10, 250, 150, 50), "Switch Scene"))
                Addressables.LoadSceneAsync(_sceneRef);
        }
        [SerializeField] private AssetReference _sceneRef;
        [SerializeField] private float _spawnRadius = 20f;
        [SerializeField] private int _spawnCount = 20;
        private readonly List<GameObject> _spawned = new();
    }
}