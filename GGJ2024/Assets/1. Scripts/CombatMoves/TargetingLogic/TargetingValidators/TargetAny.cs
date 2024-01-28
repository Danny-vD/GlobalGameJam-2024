using CombatMoves.TargetingLogic.Interfaces;
using UnityEngine;
using VDFramework;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
	public class TargetAny : BetterMonoBehaviour, ITargetingValidator
	{
		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			return true;
		}
	}
}