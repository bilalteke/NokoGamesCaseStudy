using DG.Tweening;
using NokoGames.Core;
using NokoGames.Stack;
using NokoGames.Storage;
using UnityEngine;

namespace NokoGames.Storage
{
    public abstract class GridStorageBase : StorageBase
    {
        [Header("Grid Layout")]
        [SerializeField] private int columnCount = 2;
        [SerializeField] private int rowCount = 2;
        [SerializeField] private float itemSpacingX = 0.6f;
        [SerializeField] private float itemSpacingZ = 0.6f;
        [SerializeField] private float itemSpacingY = 0.3f;

        [Header("References")]
        [SerializeField] private Transform _gridOrigin;

        public Transform GridOrigin => _gridOrigin;

        protected StackItem[] Slots;

        protected virtual void Awake()
        {
            Slots = new StackItem[maxCapacity];
        }

        public override bool IsFull
        {
            get { foreach (var s in Slots) if (s == null) return false; return true; }
        }

        public override bool IsEmpty
        {
            get { foreach (var s in Slots) if (s != null) return false; return true; }
        }

        public override bool TryAdd(StackItem item)
        {
            int idx = GetEmptySlotIndex();
            if (idx == -1) return false;

            item.transform.DOKill();
            item.transform.SetParent(null);

            Slots[idx] = item;
            Items.Add(item);

            MoveToSlot(item, idx);
            return true;
        }

        public override StackItem TryRemove()
        {
            for (int i = Slots.Length - 1; i >= 0; i--)
            {
                if (Slots[i] != null && !Slots[i].IsAnimating)
                {
                    StackItem item = Slots[i];
                    Slots[i] = null;
                    Items.Remove(item);
                    item.transform.DOKill();
                    item.transform.SetParent(null);
                    return item;
                }
            }
            return null;
        }

        public int GetEmptySlotIndex()
        {
            for (int i = 0; i < Slots.Length; i++)
                if (Slots[i] == null) return i;
            return -1;
        }

        public Vector3 GetLocalGridPosition(int index)
        {
            int itemsPerLayer = columnCount * rowCount;
            int layer = index / itemsPerLayer;
            int indexInLayer = index % itemsPerLayer;
            int row = indexInLayer % rowCount;      // önce row
            int col = indexInLayer / rowCount;      // sonra col

            return new Vector3(
                -(col * itemSpacingX),
                layer * itemSpacingY,
                -(row * itemSpacingZ)
            );
        }

        protected virtual void MoveToSlot(StackItem item, int index)
        {
            Vector3 localPos = GetLocalGridPosition(index);
            Vector3 worldPos = _gridOrigin.TransformPoint(localPos);

            item.SetAnimating(true);

            item.transform.FlyToSlot(worldPos, localPos, _gridOrigin,
                item.OriginalScale, item.OriginalRotation, onComplete: () =>
                {
                    item.SetAnimating(false);
                });
        }
    }
}