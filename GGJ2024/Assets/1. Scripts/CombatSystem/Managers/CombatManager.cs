using System.Collections.Generic;
using System.Linq;
using CharacterScripts;
using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using CombatSystem.Events.EnterCombatArenaEvent;
using InputScripts;
using InputScripts.Enums;
using PlayerPartyScripts;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.UnityExtensions;

namespace CombatSystem.Managers
{
	public class CombatManager : BetterMonoBehaviour
	{
		// Start is called before the first frame update

		public static List<GameObject> CombatParticipants { get; private set; } = new List<GameObject>();

		private void OnEnable()
		{
			EventManager.AddListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat, 100);
			EventManager.AddListener<CombatEndedEvent>(OnCombatEnd);
			
			EventManager.AddListener<EnterCombatArenaEvent>(OnCombatStart, 100);
			
		}

		private void OnDisable()
		{
			EventManager.RemoveListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);
			EventManager.RemoveListener<CombatEndedEvent>(OnCombatEnd);
			EventManager.RemoveListener<EnterCombatArenaEvent>(OnCombatStart);
		}

		private void OnCombatStart(EnterCombatArenaEvent enterCombatArenaEvent)
		{
			CombatParticipants.Clear();

			foreach (GameObject eventEnemy in enterCombatArenaEvent.Enemies)
			{
				CombatParticipants.Add(eventEnemy);
				eventEnemy.GetComponent<CharacterStateManager>().Enable();
			}

			foreach (GameObject partyMember in PlayerPartySingleton.Instance.Party.Where(partyMember => !partyMember.GetComponent<CharacterHealth>().IsDead))
			{
				CombatParticipants.Add(partyMember);
			}
			
			// TODO: move to another class
			InputControlManager.Instance.ChangeControls(ControlTypes.Combat);
		}

		private void OnCharacterEnterCombat(CharacterEnterCombatEvent characterEnterCombatEvent)
		{
			if (CombatParticipants.Contains(characterEnterCombatEvent.Character))
			{
				return;
			}

			CombatParticipants.Add(characterEnterCombatEvent.Character);
		}

		private static void OnCombatEnd(CombatEndedEvent @event)
		{
			// TODO: move to another class
			InputControlManager.Instance.ChangeControls(ControlTypes.Overworld);
		}
	}
}