using System;

namespace CombatMoves.TargetingLogic.Enums
{
    [Flags]
    public enum ValidTargets : uint
    {
        Self = 1 << 0,
        TeamMates = 1 << 1,
        Any = ~0U, // Same as enabling all others

        // Combinations
        OwnTeam = Self | TeamMates,
        Other = ~Self,
        OpposingTeam = ~OwnTeam
    }
}