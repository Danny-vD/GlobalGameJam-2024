using CombatMoves.ScriptableObjects.BaseClasses;
using LootTables.ScriptableObjects;
using UnityEngine;

namespace CombatSystem.ScriptableObjects
{
	[CreateAssetMenu(fileName = "AIMoveset", menuName = "AIMoveset")]
	public class AIMovesetLootTableObject : WeightedLootTableObject<AbstractCombatMove>
	{
	}
}
