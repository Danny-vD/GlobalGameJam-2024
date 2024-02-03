using CombatMoves.TargetingLogic.Interfaces;
using UnityEngine;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
	public class TargetEither : ITargetingValidator
	{
		private readonly ITargetingValidator[] targetingValidators;
		
		public TargetEither(params ITargetingValidator[] validators)
		{
			targetingValidators = validators;
		}
		
		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			for (int i = 0; i < targetingValidators.Length; i++)
			{
				if (targetingValidators[i].IsValidTarget(target, caster))
				{
					return true;
				}
			}

			return false;
		}
	}
}