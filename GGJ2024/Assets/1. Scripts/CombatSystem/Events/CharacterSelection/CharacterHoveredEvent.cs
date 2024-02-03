using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events.CharacterSelection
{
	public class CharacterHoveredEvent : VDEvent<CharacterHoveredEvent>
	{
		public GameObject Character;

		public CharacterHoveredEvent(GameObject character)
		{
			Character = character;
		}
	}
}