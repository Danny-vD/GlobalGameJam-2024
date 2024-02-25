using CombatSystem.CharacterScripts.CharacterStates;
using VDFramework.EventSystem;

namespace CombatSystem.Events.Queues
{
    public class NewCharacterReadyToCastEvent : VDEvent<NewCharacterReadyToCastEvent>
    {
        public readonly CastingState CastingState;

        public NewCharacterReadyToCastEvent(CastingState castingState)
        {
            CastingState = castingState;
        }
    }
}