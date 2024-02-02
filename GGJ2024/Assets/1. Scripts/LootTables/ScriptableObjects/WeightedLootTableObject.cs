using System.Collections.Generic;
using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework.LootTables;

namespace LootTables.ScriptableObjects
{
	public abstract class WeightedLootTableObject<TLootType> : LootTableObject<TLootType>
	{
		[SerializeField]
		private SerializableDictionary<TLootType, long> weightedLootTable;

		[SerializeField, Tooltip("This is part of the lootTable, use this for putting a lootTable inside a lootTable")]
		private SerializableDictionary<LootTableObject<TLootType>, long> nestedLootTables;

		protected override WeightedLootTable<TLootType> GetNewLootTable()
		{
			WeightedLootTable<TLootType> table = new WeightedLootTable<TLootType>(weightedLootTable);

			foreach (KeyValuePair<LootTableObject<TLootType>, long> pair in nestedLootTables)
			{
				table.TryAdd(pair.Key, pair.Value);
			}

			return table;
		}
		
		public bool TryAdd(KeyValuePair<LootTableObject<TLootType>, long> pair)
		{
			if (!nestedLootTables.Contains(pair))
			{
				nestedLootTables.Add(pair);

				lootTable = null; // Reset the cached loot table
				return true;
			}

			return false;
		}
		
		public bool TryAdd(LootTableObject<TLootType> loot, long weight)
		{
			return TryAdd(new KeyValuePair<LootTableObject<TLootType>, long>(loot, weight));
		}

		public bool TryAdd(KeyValuePair<TLootType, long> pair)
		{
			if (!weightedLootTable.Contains(pair))
			{
				weightedLootTable.Add(pair);

				lootTable = null; // Reset the cached loot table
				return true;
			}

			return false;
		} 
		
		public bool TryAdd(TLootType loot, long weight)
		{
			return TryAdd(new KeyValuePair<TLootType, long>(loot, weight));
		}

		public bool TryRemove(KeyValuePair<TLootType, long> pair)
		{
			if (!weightedLootTable.Contains(pair))
			{
				return false;
			}

			weightedLootTable.Remove(pair);
			return true;
		}
		
		public bool TryRemove(TLootType loot)
		{
			return TryRemove(new KeyValuePair<TLootType, long>(loot, 0)); // Weight does not matter, SerializableKeyValuePairs are equal by their Key only
		}
	}
}