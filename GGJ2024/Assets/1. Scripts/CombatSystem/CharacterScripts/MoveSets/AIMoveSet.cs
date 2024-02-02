using System.Collections.Generic;
using System.Linq;
using CombatMoves.BaseClasses;
using CombatSystem.Interfaces;
using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework.LootTables;
using VDFramework.LootTables.LootTableItems;
using VDFramework.LootTables.Variations;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public class AIMoveSet : AbstractMoveset, IAIMoveset
	{
		[SerializeField, Tooltip("If no percentage is provided when adding a new move, this percentage will be used instead")]
		private long defaultWeightForNewMove = 5;
		
		[SerializeField]
		private SerializableDictionary<AbstractCombatMove, long> moveChances;

		private WeightedLootTable<AbstractCombatMove> combatMovesLootTable;

		private void Awake()
		{
			combatMovesLootTable = new WeightedLootTable<AbstractCombatMove>(moveChances);
		}

		public void AddMove(AbstractCombatMove abstractCombatMove, long weight)
		{
			if (!moveChances.TryAdd(abstractCombatMove, weight))
			{
				return;
			}

			combatMovesLootTable.TryAdd(new LootTableItem<AbstractCombatMove>(abstractCombatMove), weight);
		}
		
		public override void AddMove(AbstractCombatMove abstractCombatMove)
		{
			AddMove(abstractCombatMove, defaultWeightForNewMove);
		}

		public override void RemoveMove(AbstractCombatMove abstractCombatMove)
		{
			moveChances.Remove(abstractCombatMove);
			
			combatMovesLootTable.TryRemove(abstractCombatMove);
		}

		public override List<AbstractCombatMove> GetMoves()
		{
			return moveChances.Keys.ToList();
		}

		public AbstractCombatMove ChooseAIMove()
		{
			return combatMovesLootTable.GetLoot();
		}
	}
}