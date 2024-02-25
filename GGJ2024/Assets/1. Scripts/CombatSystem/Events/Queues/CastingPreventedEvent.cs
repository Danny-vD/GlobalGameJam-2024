using CombatSystem.CharacterScripts.CharacterStates;
using VDFramework.EventSystem;

namespace CombatSystem.Events.Queues
{
    public class CastingPreventedEvent : VDEvent<CastingPreventedEvent>
    {
        public readonly CastingState CastingState;

        public CastingPreventedEvent(CastingState castingState)
        {
            CastingState = castingState;
        }
    }
}