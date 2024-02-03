using CombatMoves.TargetingLogic.Interfaces;
using UnityEngine;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
	public class TargetNone : ITargetingValidator
	{
		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			return false;
		}
	}
}