using System;
using System.Collections.Generic;
using CombatSystem.Events;
using CombatSystem.Events.CharacterSelection;
using PlayerPartyScripts;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Managers.TargettingSystem
{
	public class CombatTargettingManager : BetterMonoBehaviour
	{
		private List<GameObject> characterList = new List<GameObject>();

		private GameObject currentSelectedCharacter;

		private CombatManager combatManager;

		private void Awake()
		{
			combatManager = GetComponent<CombatManager>();

			EventManager.AddListener<CombatStartedEvent>(OnCombatStart);
			EventManager.AddListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);

			EventManager.AddListener<CharacterHoveredEvent>(OnCharacterHovered);
			
			// EventManager.AddListener<>();
		}

		private void Start()
		{
			InputControlManager.Instance.playerControls.Combat.MoveConfirm.performed += OnCharacterSelectConfirm;
		}

		private void OnCharacterHovered(CharacterHoveredEvent @event)
		{
			if (!characterList.Contains(@event.Character)) return;

			Debug.Log("CHARACTER SELECTED " + @event.Character.gameObject.name);
			currentSelectedCharacter = @event.Character.gameObject;
		}

		private void OnCharacterHoverExit()
		{
			currentSelectedCharacter = null;
		}

		private void OnCharacterSelectConfirm(InputAction.CallbackContext obj)
		{
			
		}
		
		private void OnCombatStart(CombatStartedEvent @event)
		{
			characterList.Clear();

			foreach (GameObject eventEnemy in @event.Enemies)
			{
				characterList.Add(eventEnemy);
			}

			foreach (GameObject partyMember in PlayerPartySingleton.Instance.Party)
			{
				characterList.Add(partyMember);
			}
		}

		private void OnCharacterEnterCombat(CharacterEnterCombatEvent @event)
		{
			if (characterList.Contains(@event.Character)) return;

			characterList.Add(@event.Character);
		}
	}
}