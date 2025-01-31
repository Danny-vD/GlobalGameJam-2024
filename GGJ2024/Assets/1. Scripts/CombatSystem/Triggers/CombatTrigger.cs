using System.Collections.Generic;
using System.Linq;
using CharacterScripts;
using CombatSystem.Enums;
using CombatSystem.Events;
using CombatSystem.Events.EnterCombatArenaEvent;
using PlayerPartyScripts;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.UnityExtensions;

namespace CombatSystem.Triggers
{
	public class CombatTrigger : BetterMonoBehaviour
	{
		[SerializeField]
		private Collider combatCollider;

		[SerializeField]
		private List<GameObject> Enemies;

		private void OnTriggerEnter(Collider other)
		{
			EventManager.RaiseEvent(new EnterCombatArenaEvent(
				PlayerPartySingleton.Instance.Party.Where(party => !party.GetComponent<CharacterHealth>().IsDead).ToList(), 
				Enemies,
				ArenaTypes.Castle_Outdoors,
				CombatTypes.Default)
			);
			
			combatCollider.Disable();
			this.Disable();
		}
	}
}