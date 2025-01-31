using System.Collections.Generic;
using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework.LootTables;
using VDFramework.LootTables.Variations;

namespace LootTables.ScriptableObjects
{
    public abstract class PercentageLootTableObject<TLootType> : LootTableObject<TLootType>
    {
        [SerializeField] private SerializableDictionary<TLootType, float> percentageLootTable;

        [SerializeField] [Tooltip("This is part of the lootTable, use this for putting a lootTable inside a lootTable")]
        private SerializableDictionary<LootTableObject<TLootType>, float> nestedLootTables;

        protected override WeightedLootTable<TLootType> GetNewLootTable()
        {
            PercentageLootTable<TLootType> weightedLootTable = new PercentageLootTable<TLootType>(percentageLootTable);

            foreach (KeyValuePair<LootTableObject<TLootType>, float> pair in nestedLootTables) weightedLootTable.TryAdd(pair.Key, pair.Value);

            return weightedLootTable;
        }

        public bool TryAdd(KeyValuePair<LootTableObject<TLootType>, float> pair)
        {
            if (!nestedLootTables.Contains(pair))
            {
                nestedLootTables.Add(pair);

                lootTable = null; // Reset the cached loot table
                return true;
            }

            return false;
        }

        public bool TryAdd(LootTableObject<TLootType> loot, float percentage)
        {
            return TryAdd(new KeyValuePair<LootTableObject<TLootType>, float>(loot, percentage));
        }

        public bool TryAdd(KeyValuePair<TLootType, float> pair)
        {
            if (!percentageLootTable.Contains(pair))
            {
                percentageLootTable.Add(pair);

                lootTable = null; // Reset the cached loot table
                return true;
            }

            return false;
        }

        public bool TryAdd(TLootType loot, float percentage)
        {
            return TryAdd(new KeyValuePair<TLootType, float>(loot, percentage));
        }

        public bool TryRemove(KeyValuePair<TLootType, float> pair)
        {
            if (!percentageLootTable.Contains(pair)) return false;

            percentageLootTable.Remove(pair);
            return true;
        }

        public bool TryRemove(TLootType loot)
        {
            return
                TryRemove(new KeyValuePair<TLootType, float>(loot,
                    0)); // percentage does not matter, SerializableKeyValuePairs are equal by their Key only
        }
    }
}