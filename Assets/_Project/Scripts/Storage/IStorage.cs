using System.Collections;
using System.Collections.Generic;
using NokoGames.Stack;
using UnityEngine;

namespace NokoGames.Storage
{
    public interface IStorage
    {
        bool IsFull { get; }
        bool IsEmpty { get; }
        int Count { get; }
        bool TryAdd(StackItem item);
        StackItem TryRemove();
        bool Accepts(ItemDefinition definition);
    }
}
