using System.Collections.Generic;
using CombatMoves.TargetingLogic.Enums;
using CombatMoves.TargetingLogic.Interfaces;

namespace CombatMoves.TargetingLogic.TargetingValidators.Util
{
	public static class TargetingValidatorUtil
	{
		public static List<ITargetingValidator> GetValidators(ValidTargets validTargets)
		{
			List<ITargetingValidator> validators = new List<ITargetingValidator>();
			
			// TODO: Implement this function
			//TEMP
			validators.Add(new TargetAny());
			
			return validators;
		}
	}
}