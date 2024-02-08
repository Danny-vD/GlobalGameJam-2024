using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using CombatSystem.Events.CharacterSelection;
using InputScripts;
using PlayerPartyScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace CombatSystem.Managers.TargettingSystem
{
	public class CombatTargettingManager : Singleton<CombatTargettingManager> // TODO: override target selection when taunted?? (Can a player be taunted?)
	{
		[SerializeField]
		private GameObject currentSelectedCharacterIndicator;

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
			EventManager.AddListener<CombatEndedEvent>(StopListeneningToConfirmInput);

			EventManager.AddListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);
			EventManager.AddListener<CharacterHoveredEvent>(OnCharacterHovered);
		}

		public void ChooseMove(AbstractCombatMove move, GameObject caster)
		{
			if (toBeConfirmedMove != null) // NOTE temporary hotfix which prevent selecting a move after we selected one to prevent a SelectMove from being called on someone that's not in the playerTurn queue (see CombatMoveUIManager)
			{
				return;
			}
			
			// BUG: left click confirms, but left click also selects a move | we cannot select another move while we're listening
			StartListeneningToConfirmInput();

			toBeConfirmedMove = move;
			casterToBe        = caster;
		}


		private void OnCharacterHovered(CharacterHoveredEvent @event)
		{
			if (!characterList.Contains(@event.Character)) return;

			currentSelectedCharacter = @event.Character.gameObject;

			currentSelectedCharacterIndicator.transform.position = currentSelectedCharacter.transform.position;
			currentSelectedCharacterIndicator.transform.Translate(Vector3.up * 1.5f);
		}

		private void OnTargetSelectConfirm(InputAction.CallbackContext obj)
		{
			if (!toBeConfirmedMove) return;

			if (toBeConfirmedMove.IsValidTarget(currentSelectedCharacter, casterToBe))
			{
				ConfirmedMoveHolder confirmedMoveHolder = casterToBe.GetComponent<ConfirmedMoveHolder>();
				confirmedMoveHolder.SelectMove(toBeConfirmedMove, currentSelectedCharacter);
				toBeConfirmedMove = null;

				StopListeneningToConfirmInput();
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

		private void StartListeneningToConfirmInput()
		{
			InputControlManager.Instance.playerControls.Combat.MoveConfirm.performed += OnTargetSelectConfirm;
		}

		private void StopListeneningToConfirmInput()
		{
			InputControlManager.Instance.playerControls.Combat.MoveConfirm.performed -= OnTargetSelectConfirm;
		}

		protected override void OnDestroy()
		{
			EventManager.RemoveListener<CombatStartedEvent>(OnCombatStart);
			EventManager.RemoveListener<CombatEndedEvent>(StopListeneningToConfirmInput);

			EventManager.RemoveListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);
			EventManager.RemoveListener<CharacterHoveredEvent>(OnCharacterHovered);

			base.OnDestroy();
		}
	}
}