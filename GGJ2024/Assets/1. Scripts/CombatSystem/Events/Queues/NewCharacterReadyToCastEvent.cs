using CombatSystem.CharacterScripts.CharacterStates;
using VDFramework.EventSystem;

namespace CombatSystem.Events.Queues
{
	public class NewCharacterReadyToCastEvent : VDEvent<NewCharacterReadyToCastEvent>
	{
		public readonly CastingState ReadyCastingState;

		public NewCharacterReadyToCastEvent(CastingState castingState)
		{
			ReadyCastingState = castingState;
		}
	}
}