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
				ValidTargets.Any => new TargetAny(),
				ValidTargets.Self => new TargetSelf(),
				ValidTargets.Other => new TargetOther(),
				ValidTargets.TeamMates => new TargetCombined(new TargetOther(), new TargetOwnTeam()),
				ValidTargets.OwnTeam => new TargetOwnTeam(),
				ValidTargets.OpposingTeam => new TargetOpposingTeam(),
				_ => null,
			};

			if (validator != null)
			{
				return validator;
			}

			return GetCombinedValidator(validTargets);
		}

		private static ITargetingValidator GetCombinedValidator(ValidTargets validTargets)
		{
			List<ITargetingValidator> targetingValidators = new List<ITargetingValidator>();

			if ((validTargets & ValidTargets.Self) != 0)
			{
				targetingValidators.Add(new TargetSelf());
			}
			else if ((validTargets & ValidTargets.Other) != 0) // Else because 'Self' and 'Other' are mutually exclusive, if you have both flags then you can target 'Any'
			{
				targetingValidators.Add(new TargetOther());
			}

			if ((validTargets & ValidTargets.TeamMates) != 0)
			{
				targetingValidators.Add(new TargetCombined(new TargetOther(), new TargetOwnTeam()));
			}

			if ((validTargets & ValidTargets.OpposingTeam) != 0)
			{
				targetingValidators.Add(new TargetOpposingTeam());
			}

			return new TargetCombined(targetingValidators.ToArray());
		}
	}
}