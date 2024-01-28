using System;

namespace CombatMoves.TargetingLogic.Enums
{
	[Flags]
	public enum ValidTargets
	{
		Self = 1 << 0,
		Other = ~Self,
		Teammates = 1 << 1,
		OwnTeam = Self | Teammates,
		OpposingTeam = 1 << 2,
		Any = OwnTeam | OpposingTeam,
	}
}