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

		public override List<AbstractCombatMove> GetMoves()
		{
			return moveChances.Keys.ToList();
		}

		public AbstractCombatMove ChooseAIMove()
		{
			return percentagedMoves.GetLoot();
		}

		public override T AddMove<T>()
		{
			return AddMove<T>(defaultPercentageForNewMove);
		}

		public T AddMove<T>(float percentage) where T : AbstractCombatMove
		{
			T combatMove = base.AddMove<T>();


			percentagedMoves.Add(new LootTableItem<T>(combatMove), percentage);

			return combatMove;
		}
	}
}