using NokoGames.Core;
using UnityEngine;

namespace NokoGames.VFX
{
    public class FloatingTextSpawner : MonoBehaviour
    {
        public static FloatingTextSpawner Instance { get; private set; }

        [SerializeField] private FloatingText _prefab;
        [SerializeField] private int _poolSize = 10;

        private ObjectPool<FloatingText> _pool;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            _pool = new ObjectPool<FloatingText>(_prefab, transform, _poolSize);
        }

        public void Show(string message, Vector3 worldPos)
        {
            FloatingText ft = _pool.Get();
            ft.Play(message, worldPos, () => _pool.Return(ft));
        }
    }
}