using DG.Tweening;
using NokoGames.Core;
using NokoGames.Stack;
using NokoGames.Storage;
using UnityEngine;

namespace NokoGames.Transformer
{
    public class TransformerOutputStorage : GridStorageBase
    {
        [Header("Spawn Point")]
        [SerializeField] private Transform _spawnPoint;

        public override bool TryAdd(StackItem item)
        {
            int idx = GetEmptySlotIndex();
            if (idx == -1) return false;

            item.transform.SetParent(null);
            item.transform.position = _spawnPoint.position;
            item.transform.rotation = GridOrigin.rotation * item.OriginalRotation;
            item.transform.localScale = Vector3.zero;

            Slots[idx] = item;
            Items.Add(item);

            MoveToSlot(item, idx);
            return true;
        }

        protected override void MoveToSlot(StackItem item, int index)
        {
            Vector3 localPos = GetLocalGridPosition(index);
            Vector3 worldPos = GridOrigin.TransformPoint(localPos);

            item.SetAnimating(true);

            item.transform.PopAndFlyToSlot(worldPos, localPos, GridOrigin,
                item.OriginalScale, item.OriginalRotation, onComplete: () =>
                {
                    item.SetAnimating(false);
                });
        }
    }
}