using System.Collections.Generic;
using System.Linq;
using CombatMoves.BaseClasses;
using CombatSystem.Interfaces;
using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework.LootTables.LootTableItems;
using VDFramework.LootTables.Variations;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public class AIMoveSet : AbstractMoveset, IAIMoveset
	{
		[SerializeField, Tooltip("If no percentage is provided when adding a new move, this percentage will be used instead")]
		private float defaultPercentageForNewMove = 5;
		
		[SerializeField]
		private SerializableDictionary<AbstractCombatMove, float> moveChances;

		private PercentageLootTable<AbstractCombatMove> percentagedMoves;

		private void Awake()
		{
			percentagedMoves = new PercentageLootTable<AbstractCombatMove>(moveChances);
		}

		public void AddMove(AbstractCombatMove abstractCombatMove, float percentage)
		{
			if (!moveChances.TryAdd(abstractCombatMove, percentage))
			{
				return;
			}

			percentagedMoves.TryAdd(new LootTableItem<AbstractCombatMove>(abstractCombatMove), percentage);
		}
		
		public override void AddMove(AbstractCombatMove abstractCombatMove)
		{
			AddMove(abstractCombatMove, defaultPercentageForNewMove);
		}

		public override void RemoveMove(AbstractCombatMove abstractCombatMove)
		{
			moveChances.Remove(abstractCombatMove);
			
			percentagedMoves.TryRemove(abstractCombatMove);
		}

		public override List<AbstractCombatMove> GetMoves()
		{
			return moveChances.Keys.ToList();
		}

		public AbstractCombatMove ChooseAIMove()
		{
			return percentagedMoves.GetLoot();
		}
	}
}