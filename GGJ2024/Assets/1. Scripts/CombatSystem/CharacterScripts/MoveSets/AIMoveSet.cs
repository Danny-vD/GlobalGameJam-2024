using System.Collections.Generic;
using System.Linq;
using CombatMoves.ScriptableAssets;
using CombatSystem.Interfaces;
using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework;
using VDFramework.LootTables.Variations;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public class AIMoveSet : BetterMonoBehaviour, IAIMoveset
	{
		[SerializeField]
		private SerializableDictionary<CombatMove, float> moves;

		private PercentageLootTable<CombatMove> percentagedMoves;

		private void Awake()
		{
			percentagedMoves = new PercentageLootTable<CombatMove>(moves);
		}

		public List<CombatMove> GetMoves()
		{
			return moves.Keys.ToList();
		}

		public CombatMove ChooseAIMove()
		{
			return percentagedMoves.GetLoot();
		}
	}
}