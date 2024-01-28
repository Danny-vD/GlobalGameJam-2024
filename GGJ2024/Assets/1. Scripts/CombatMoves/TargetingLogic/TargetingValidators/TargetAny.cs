using CombatMoves.TargetingLogic.Interfaces;
using UnityEngine;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
	public class TargetAny : ITargetingValidator
	{
		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			return true;
		}
	}
}