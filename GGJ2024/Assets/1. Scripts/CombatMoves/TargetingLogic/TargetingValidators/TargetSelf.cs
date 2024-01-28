using CombatMoves.TargetingLogic.Interfaces;
using UnityEngine;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
	public class TargetSelf : ITargetingValidator
	{
		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			return ReferenceEquals(target, caster);
		}
	}
}