using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events.CharacterStateEvents
{
	public class PlayerStartedChoosingEvent : VDEvent<PlayerStartedChoosingEvent>
	{
		public readonly GameObject Player;

		public PlayerStartedChoosingEvent(GameObject player)
		{
			Player = player;
		}
	}
}