using CombatMoves.TargetingLogic.Interfaces;
using UnityEngine;
using VDFramework;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
	public class TargetOther : BetterMonoBehaviour, ITargetingValidator
	{
		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			return !ReferenceEquals(target, caster);
		}
	}
}