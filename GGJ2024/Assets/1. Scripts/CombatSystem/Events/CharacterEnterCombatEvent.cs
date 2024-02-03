using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events
{
	public class CharacterEnterCombatEvent : VDEvent<CharacterEnterCombatEvent>
	{
		public GameObject Character;

		public CharacterEnterCombatEvent(GameObject character)
		{
			Character = character;
		}
	}
}