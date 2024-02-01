using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events.CharacterStateEvents
{
	public class PlayerStoppedChoosingEvent : VDEvent<PlayerStoppedChoosingEvent>
	{
		public GameObject Player;

		public PlayerStoppedChoosingEvent(GameObject player)
		{
			Player = player;
		}
	}
}