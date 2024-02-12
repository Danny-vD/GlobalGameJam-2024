using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using UnityEngine;
using VDFramework.Extensions;

namespace AI
{
	public class RandomTargetLogic : AbstractAITargetingLogic
	{
		public override List<GameObject> GetTargets(AbstractCombatMove combatMove)
		{
			List<GameObject> targets = new List<GameObject> { GetAllValidTargets(combatMove).GetRandomElement() };
			return targets;
		}
	}
}