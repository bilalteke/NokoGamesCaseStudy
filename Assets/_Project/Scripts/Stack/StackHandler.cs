using System.Collections.Generic;
using DG.Tweening;
using NokoGames.Storage;
using UnityEngine;

namespace NokoGames.Stack
{
    public class StackHandler : MonoBehaviour
    {
        [Header("Stack Settings")]
        [SerializeField] private int maxCapacity = 10;
        [SerializeField] private Transform stackAnchor;

        [Header("Animation Settings")]
        [SerializeField] private float moveSpeed = 0.2f;
        [SerializeField] private float arcHeight = 0.5f;

        [Header("Swing Settings")]
        [SerializeField] private float followSpeed = 0.05f;
        [SerializeField] private float rotationSpeed = 0.15f;

        [Header("Interaction Settings")]
        [SerializeField] private float _pickupInterval = 0.3f;
        [SerializeField] private float _depositInterval = 0.3f;

        public float PickupInterval => _pickupInterval;
        public float DepositInterval => _depositInterval;

        private readonly List<StackItem> _items = new List<StackItem>();

        public bool IsFull => _items.Count >= maxCapacity;
        public bool IsEmpty => _items.Count == 0;
        public int Count => _items.Count;

        private Transform _itemContainer;

        private void Awake()
        {
            _itemContainer = new GameObject($"{gameObject.name}_Stack").transform;
        }

        public bool TryAdd(StackItem item)
        {
            if (IsFull) return false;

            item.transform.DOKill();
            item.transform.SetParent(_itemContainer);
            item.transform.rotation = Quaternion.identity;

            _items.Add(item);
            item.OnPickedUp();
            AnimateToPosition(item, _items.Count - 1);
            return true;
        }

        public StackItem TryRemove()
        {
            if (IsEmpty) return null;

            StackItem item = _items[_items.Count - 1];
            _items.RemoveAt(_items.Count - 1);
            item.transform.DOKill();
            item.transform.SetParent(null);
            item.OnDropped();
            return item;
        }

        public StackItem Peek()
        {
            if (IsEmpty) return null;
            return _items[_items.Count - 1];
        }

        public StackItem GetItemAt(int index)
        {
            if (index < 0 || index >= _items.Count) return null;
            return _items[index];
        }

        /// <summary>
        /// Üstten alta tarar, eşleşen definition'daki ilk item'ı çıkarır.
        /// </summary>
        public StackItem TryRemoveByDefinition(ItemDefinition definition)
        {
            if (definition == null) return null;

            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i].ItemDefinition != definition) continue;

                StackItem item = _items[i];
                _items.RemoveAt(i);
                item.transform.DOKill();
                item.transform.SetParent(null);
                item.OnDropped();
                return item;
            }
            return null;
        }

        private void AnimateToPosition(StackItem item, int index)
        {
            Vector3 targetPos = stackAnchor.position + stackAnchor.up * (index * item.StackSpacing);
            item.transform.DOJump(targetPos, arcHeight, 1, moveSpeed)
                .SetEase(Ease.OutQuad);
        }

        private void FixedUpdate()
        {
            if (IsEmpty) return;
            for (int i = 0; i < _items.Count; i++)
            {
                Transform targetRef = (i == 0) ? stackAnchor : _items[i - 1].transform;
                _items[i].Follow(targetRef, followSpeed, rotationSpeed, _items[i].StackSpacing);
            }
        }

        public ItemDefinition FindAcceptedDefinition(IStorage storage)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                ItemDefinition def = _items[i].ItemDefinition;
                if (storage.Accepts(def)) return def;
            }
            return null;
        }
    }
}