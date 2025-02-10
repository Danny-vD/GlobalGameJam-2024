namespace CombatMoves.TargetingLogic.Enums
{
    public enum TargetingMode
    {
        TargetAll, // Will auto-target all valid targets
        SingleTargetOnly, // Can only target a single character
        MultipleTargets, // Can target multiple, but does not *have* to
        MultipleTargetsOnly, // Can only target multiple characters
        None, // Cannot target anything (used for e.g. 'wait')
    }
}