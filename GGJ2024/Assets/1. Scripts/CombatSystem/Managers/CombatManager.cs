using System.Collections.Generic;
using System.Linq;
using CharacterScripts;
using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using CombatSystem.Events.EnterCombatArenaEvent;
using InputScripts;
using InputScripts.Enums;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.UnityExtensions;

namespace CombatSystem.Managers
{
	public class CombatManager : BetterMonoBehaviour // NOTE: Can probably be turned into a static class
	{
		public static List<GameObject> CombatParticipants { get; private set; } = new List<GameObject>();

		public static IEnumerable<GameObject> GetAliveCombatParticipants()
		{
			return CombatParticipants.Where(participant => !participant.GetComponent<CharacterHealth>().IsDead);
		}
		
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

			foreach (GameObject partyMember in enterCombatArenaEvent.PartyMembers)
			{
				CombatParticipants.Add(partyMember);
			}

			// TODO: move to another class
			InputControlManager.Instance.ChangeControls(ControlTypes.Combat);
		}

		private static void OnCharacterEnterCombat(CharacterEnterCombatEvent characterEnterCombatEvent)
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