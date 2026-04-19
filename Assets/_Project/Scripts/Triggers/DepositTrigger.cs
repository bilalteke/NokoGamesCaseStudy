using NokoGames.AI;
using NokoGames.Stack;
using NokoGames.Storage;
using UnityEngine;

namespace NokoGames.Triggers
{
    [RequireComponent(typeof(Collider))]
    public class DepositTrigger : MonoBehaviour, IAIZone
    {
        [SerializeField] private float _depositInterval = 0.3f;

        private GridStorageBase _storage;
        private float _nextDepositTime;

        public Transform Destination => transform;
        public bool IsInsideZone { get; private set; }
        public GridStorageBase Storage => _storage;

        private void Awake()
        {
            _storage = GetComponentInParent<GridStorageBase>();
            if (_storage == null)
                Debug.LogError($"[DepositTrigger] GridStorageBase bulunamadı: {gameObject.name}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out AIController _)) IsInsideZone = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out AIController _)) IsInsideZone = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out AIController _)) return;
            if (!other.TryGetComponent(out StackHandler stackHandler)) return;
            if (Time.time < _nextDepositTime) return;
            if (stackHandler.IsEmpty) return;
            if (_storage.IsFull) return;

            ItemDefinition accepted = stackHandler.FindAcceptedDefinition(_storage);
            if (accepted == null) return;

            StackItem item = stackHandler.TryRemoveByDefinition(accepted);
            if (item == null) return;

            bool added = _storage.TryAdd(item);
            if (!added) { stackHandler.TryAdd(item); return; }

            _nextDepositTime = Time.time + stackHandler.DepositInterval;
        }
    }
}