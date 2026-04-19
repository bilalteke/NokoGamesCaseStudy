using NokoGames.Stack;
using NokoGames.Storage;

namespace NokoGames.Spawner
{
    public class SpawnerOutputStorage : GridStorageBase
    {
        public override bool TryAdd(StackItem item)
        {
            int idx = GetEmptySlotIndex();
            if (idx == -1) return false;

            Slots[idx] = item;
            Items.Add(item);
            return true;
        }

        public int GetEmptySlotCount()
        {
            int count = 0;
            for (int i = 0; i < Slots.Length; i++)
                if (Slots[i] == null) count++;
            return count;
        }
    }
}