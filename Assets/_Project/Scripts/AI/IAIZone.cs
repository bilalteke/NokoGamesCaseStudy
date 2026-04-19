using NokoGames.Storage;
using UnityEngine;

namespace NokoGames.AI
{
    public interface IAIZone
    {
        Transform Destination { get; }
        bool IsInsideZone { get; }
        GridStorageBase Storage { get; }
    }
}