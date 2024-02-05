using UnityEngine;

namespace ZBase.Foundation.Pooling.GameObjectItem.LazyPool
{
    public class PoolItemAutoDeSpawnSetUp : MonoBehaviour
    {
        [SerializeField] private float _lifeTime;
        public float LifeTime => _lifeTime;
    }
}