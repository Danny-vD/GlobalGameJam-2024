using CombatSystem.CharacterScripts.CharacterStates;
using VDFramework.EventSystem;

namespace CombatSystem.Events.Queues
{
    public class CastingInterupptedEvent : VDEvent<CastingInterupptedEvent>
    {
        public readonly CastingState CastingState;

        public CastingInterupptedEvent(CastingState castingState)
        {
            CastingState = castingState;
        }
    }
}