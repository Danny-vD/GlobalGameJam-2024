using System.Collections.Generic;
using CombatMoves.TargetingLogic.Enums;
using CombatMoves.TargetingLogic.Interfaces;

namespace CombatMoves.TargetingLogic.TargetingValidators.Util
{
	public static class TargetingValidatorUtil
	{
		public static ITargetingValidator GetValidators(ValidTargets validTargets)
		{
			ITargetingValidator validator = validTargets switch
			{
				0 => new TargetNone(),
				ValidTargets.Any => new TargetAny(),
				ValidTargets.Self => new TargetSelf(),
				ValidTargets.Other => new TargetOther(),
				ValidTargets.TeamMates => new TargetCombined(new TargetOther(), new TargetOwnTeam()),
				ValidTargets.OwnTeam => new TargetOwnTeam(),
				ValidTargets.OpposingTeam => new TargetOpposingTeam(),
				ValidTargets.Self | ValidTargets.OpposingTeam => new TargetEither(new TargetSelf(), new TargetOpposingTeam()),
				_ => null,
			};

			return validator; // ?? GetCombinedValidator(validTargets); // Self and Opposing team is the only combination that is not covered by any other value
		}

		private static ITargetingValidator GetCombinedValidator(ValidTargets validTargets)
		{
			List<ITargetingValidator> targetingValidators = new List<ITargetingValidator>();
			
			if ((validTargets & ValidTargets.Self) != 0)
			{
				targetingValidators.Add(new TargetSelf());
			}
			
			if ((validTargets & ValidTargets.OpposingTeam) != 0)
			{
				targetingValidators.Add(new TargetOpposingTeam());
			}

			return new TargetEither(targetingValidators.ToArray());
		}
	}
}