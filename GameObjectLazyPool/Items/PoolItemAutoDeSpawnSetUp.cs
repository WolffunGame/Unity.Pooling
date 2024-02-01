using UnityEngine;

namespace ZBase.Foundation.Pooling.GameObject.LazyPool
{
    public class PoolItemAutoDeSpawnSetUp : MonoBehaviour
    {
        [SerializeField] private float _lifeTime;
        public float LifeTime => _lifeTime;
    }
}