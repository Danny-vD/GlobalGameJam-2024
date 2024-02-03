using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events
{
	public class CharacterEnterCombatEvent : VDEvent<CharacterEnterCombatEvent>
	{
		public readonly GameObject Character;

		public CharacterEnterCombatEvent(GameObject character)
		{
			Character = character;
		}
	}
}