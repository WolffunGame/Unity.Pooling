using Cysharp.Threading.Tasks;
using UnityEngine;
using ZBase.Collections.Pooled.Generic;
using ZBase.Foundation.Pooling.GameObjectItem.LazyPool.Extensions;
using ZBase.Foundation.Pooling.UnityPools;

namespace Pooling.Sample
{
    public class GameObjectPoolSample : MonoBehaviour
    {
        public GameObjectPrefab prefab1;
        public GameObject prefab2;
        public int sceneID;
        
        private async UniTask Spawn()
        {
            var item = await LazyGameObjectPool.Rent(prefab1);
            item.SetActive(true);
            var pos = Random.insideUnitCircle * _spawnRadius;
            item.transform.position = new Vector3 {x = pos.x, y = 0, z = pos.y};
            _spawned.Add(item);
        } 
        
        private void SpawnByPrefab()
        {
            var item =  LazyGameObjectPool.RentSync(prefab2);
            item.SetActive(true);
            var pos = Random.insideUnitCircle * _spawnRadius;
            item.transform.position = new Vector3 {x = pos.x, y = 0, z = pos.y};
            _spawned.Add(item);
        } 


        private  void OnGUI()
        {
            //manually get the CustomGameObjectPool then rent and spawn some objects
            if (GUI.Button(new Rect(0, 0, 150, 50), "Spawn"))
                for (int i = 0; i < _spawnCount; i++)
                    Spawn().Forget();    
            if (GUI.Button(new Rect(0, 60, 150, 50), "Spawn By Prefab"))
                for (int i = 0; i < this._spawnCount; i++)
                    SpawnByPrefab();
            if (GUI.Button(new Rect(0, 120, 150, 50), "DeSpawn All"))
            {
                foreach (var go in _spawned)
                {
                    go.SetActive(false);
                    LazyGameObjectPool.Return(go);
                }
                _spawned.Clear();
            }
            if (!GUI.Button(new Rect(0, 180, 150, 50), "Switch Scene"))
                return;
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);
        }
        
        [SerializeField] private float _spawnRadius = 20f;
        [SerializeField] private int _spawnCount = 20;
        private readonly List<GameObject> _spawned = new();
    }
}