using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Events.CharacterSelection;
using CombatSystem.Managers;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace CombatSystem.Targeting
{
	public class CombatTargettingManager : Singleton<CombatTargettingManager> // TODO: override target selection when taunted?? (Can a player be taunted?)
	{
		[SerializeField]
		private GameObject currentSelectedCharacterIndicator;

		private readonly List<GameObject> selectedTargets = new List<GameObject>();
		private AbstractCombatMove currentMove;

		private PlayerTurnManager turnManager;

		private CombatManager combatManager;

		protected override void Awake()
		{
			base.Awake();

			turnManager = GetComponent<PlayerTurnManager>();
		}

		protected override void OnDestroy()
		{
			EventManager.RemoveListener<CharacterClickedEvent>(OnCharacterClicked);

			base.OnDestroy();
		}

		public void ChooseMove(AbstractCombatMove move, GameObject caster)
		{
			// BUG: left click confirms, but left click also selects a move | trying to select a move after you already selected a move simultaneously confirms the target and then selects the move (which causes you to select a move on someone who is not allowed yet to select a move)

			selectedTargets.Clear();

			currentMove = move;
		}


		private void OnCharacterClicked(CharacterClickedEvent @event)
		{
			if (!@event.Character)
			{
				currentSelectedCharacterIndicator.transform.position = Vector3.zero;
			}

			if (!CombatManager.CombatParticipants.Contains(@event.Character))
			{
#if UNITY_EDITOR
				Debug.LogError("Tried to target a character that is not in combat!");
#endif

				return;
			}

			selectedTargets.Add(@event.Character);

			//TODO move to a seperate class
			currentSelectedCharacterIndicator.transform.position = @event.Character.transform.position + Vector3.up * 1.5f;
		}

		public void OnTargetSelectConfirm()
		{
			if (selectedTargets.Count == 0)
			{
				return;
			}

			if (!turnManager.TryGetActivePlayer(out GameObject currentPlayer))
			{
				Debug.LogError("Confirmed a move but there's no active player!");
				return;
			}

			ConfirmedMoveHolder confirmedMoveHolder = currentPlayer.GetComponent<ConfirmedMoveHolder>();
			confirmedMoveHolder.SelectMove(currentMove, selectedTargets);
			currentMove = null;

			selectedTargets.Clear();
		}
	}
}