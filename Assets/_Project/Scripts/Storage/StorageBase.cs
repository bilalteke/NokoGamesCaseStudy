using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NokoGames.Stack;

namespace NokoGames.Storage
{
    public abstract class StorageBase : MonoBehaviour, IStorage
    {
        [SerializeField] protected int maxCapacity = 10;
        [SerializeField] private ItemDefinition[] _acceptedDefinitions;

        protected readonly List<StackItem> Items = new List<StackItem>();

        public virtual bool IsFull => Items.Count >= maxCapacity;
        public virtual bool IsEmpty => Items.Count == 0;
        public int Count => Items.Count;

        public virtual bool Accepts(ItemDefinition definition)
        {
            foreach (var d in _acceptedDefinitions)
                if (d == definition) return true;
            return false;
        }

        public virtual bool TryAdd(StackItem item)
        {
            if (IsFull) return false;
            Items.Add(item);
            OnItemAdded(item);
            return true;
        }

        public virtual StackItem TryRemove()
        {
            if (IsEmpty) return null;
            StackItem item = Items[Items.Count - 1];
            Items.RemoveAt(Items.Count - 1);
            OnItemRemoved(item);
            return item;
        }

        protected virtual void OnItemAdded(StackItem item) { }
        protected virtual void OnItemRemoved(StackItem item) { }
    }
}