using NokoGames.AI;
using NokoGames.Stack;
using NokoGames.Storage;
using UnityEngine;

namespace NokoGames.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class PickupTrigger : MonoBehaviour, IAIZone
    {
        private GridStorageBase _storage;
        private float _nextPickupTime;

        public Transform Destination => transform;
        public bool IsInsideZone { get; private set; }
        public GridStorageBase Storage => _storage;

        private void Awake()
        {
            _storage = GetComponentInParent<GridStorageBase>();
            if (_storage == null)
                Debug.LogError($"[PickupTrigger] GridStorageBase bulunamadı: {gameObject.name}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out AIController _))
                IsInsideZone = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out AIController _)) IsInsideZone = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out AIController _)) return;
            if (!other.TryGetComponent(out StackHandler stackHandler)) return;
            if (Time.time < _nextPickupTime) return;
            if (stackHandler.IsFull) return;
            if (_storage.IsEmpty) return;

            StackItem item = _storage.TryRemove();
            if (item == null) return;

            bool added = stackHandler.TryAdd(item);
            if (!added) { _storage.TryAdd(item); return; }

            _nextPickupTime = Time.time + stackHandler.PickupInterval;
        }
    }
}