using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NokoGames.Stack
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "NokoGames/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string _itemName;
        public string ItemName => _itemName;
    }
}
