using System.Collections.Generic;
using CombatMoves.BaseClasses.ScriptableObjects;
using CombatSystem.Interfaces;
using LootTables.ScriptableObjects;
using UnityEngine;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public class AIMoveSet : AbstractMoveset, IAIMoveset
	{
		[SerializeField]
		private WeightedLootTableObject<AbstractCombatMove> movesTable;
		
		[SerializeField, Tooltip("If no percentage is provided when adding a new move, this percentage will be used instead")]
		private long defaultWeightForNewMove = 5;

		public void AddMove(AbstractCombatMove abstractCombatMove, long weight)
		{
			if (!movesTable.TryAdd(abstractCombatMove, weight))
			{
				return;
			}
		}
		
		public override void AddMove(AbstractCombatMove abstractCombatMove)
		{
			AddMove(abstractCombatMove, defaultWeightForNewMove);
		}

		public override void RemoveMove(AbstractCombatMove abstractCombatMove)
		{
			movesTable.TryRemove(abstractCombatMove);
		}

		public override List<AbstractCombatMove> GetMoves()
		{
			return null; //movesTable.GetLootTable().GetLootList();
		}

		public AbstractCombatMove ChooseAIMove()
		{
			return movesTable.GetLoot();
		}
	}
}