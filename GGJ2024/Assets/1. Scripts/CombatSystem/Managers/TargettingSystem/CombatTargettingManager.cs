using System;
using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using CombatSystem.Events.CharacterSelection;
using PlayerPartyScripts;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace CombatSystem.Managers.TargettingSystem
{
	public class CombatTargettingManager : Singleton<CombatTargettingManager>
	{
		private List<GameObject> characterList = new List<GameObject>();

		private GameObject currentSelectedCharacter;

		private CombatManager combatManager;

		private AbstractCombatMove toBeConfirmedMove;

		private GameObject casterToBe;

		protected override void Awake()
		{
			base.Awake();
			
			combatManager = GetComponent<CombatManager>();

			EventManager.AddListener<CombatStartedEvent>(OnCombatStart);
			EventManager.AddListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);

			EventManager.AddListener<CharacterHoveredEvent>(OnCharacterHovered);

			// EventManager.AddListener<>();
		}

		public void ChooseMove(AbstractCombatMove move, GameObject caster)
		{
			InputControlManager.Instance.playerControls.Combat.MoveConfirm.performed += OnCharacterSelectConfirm;
			toBeConfirmedMove                                                        =  move;
			casterToBe                                                               =  caster;
		}
		

		private void OnCharacterHovered(CharacterHoveredEvent @event)
		{
			if (!characterList.Contains(@event.Character)) return;

			Debug.Log("CHARACTER SELECTED " + @event.Character.gameObject.name);
			currentSelectedCharacter = @event.Character.gameObject;
		}

		private void OnCharacterSelectConfirm(InputAction.CallbackContext obj)
		{
			if (!toBeConfirmedMove) return;

			//TODO: check if legal target?
			if (toBeConfirmedMove.IsValidTarget(currentSelectedCharacter, casterToBe))
			{
				casterToBe.GetComponent<ConfirmedMoveHolder>().SelectMove(toBeConfirmedMove, currentSelectedCharacter);
				
				InputControlManager.Instance.playerControls.Combat.MoveConfirm.performed -= OnCharacterSelectConfirm;
			}
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