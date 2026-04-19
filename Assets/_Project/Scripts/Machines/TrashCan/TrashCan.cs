using NokoGames.AI;
using NokoGames.Core;
using NokoGames.Currency;
using NokoGames.Stack;
using NokoGames.Storage;
using NokoGames.VFX;
using UnityEngine;

namespace NokoGames.Machines
{
    [RequireComponent(typeof(Collider))]
    public class TrashCan : MonoBehaviour, IAIZone, IStorage
    {
        [Header("Settings")]
        [SerializeField] private float _destroyInterval = 0.15f;
        [SerializeField] private float _flyDuration = 0.3f;
        [SerializeField] private float _arcHeight = 0.5f;

        [Header("Currency")]
        [SerializeField] private int _rewardPerItem = 10;

        [Header("Accepted Items")]
        [SerializeField] private ItemDefinition[] _acceptedDefinitions;

        public Transform Destination => transform;
        public bool IsInsideZone { get; private set; }
        public GridStorageBase Storage => null;

        public bool IsFull => false;
        public bool IsEmpty => true;
        public int Count => 0;
        public bool TryAdd(StackItem item) => false;
        public StackItem TryRemove() => null;

        public bool Accepts(ItemDefinition definition)
        {
            foreach (var d in _acceptedDefinitions)
                if (d == definition) return true;
            return false;
        }

        private float _nextDestroyTime;

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
            if (Time.time < _nextDestroyTime) return;
            if (!other.TryGetComponent(out StackHandler stackHandler)) return;
            if (stackHandler.IsEmpty) return;

            ItemDefinition accepted = stackHandler.FindAcceptedDefinition(this);
            if (accepted == null) return;

            StackItem item = stackHandler.TryRemoveByDefinition(accepted);
            if (item == null) return;

            DestroyItem(item);
            _nextDestroyTime = Time.time + _destroyInterval;
        }

        public void DestroyItem(StackItem item)
        {
            item.SetAnimating(true);
            item.transform.FlyAndVanish(transform.position, _flyDuration, _arcHeight, () =>
            {
                CurrencyManager.Instance?.Add(_rewardPerItem);
                FloatingTextSpawner.Instance?.Show($"+${_rewardPerItem}", transform.position + Vector3.up);
                Destroy(item.gameObject);
            });
        }
    }
}