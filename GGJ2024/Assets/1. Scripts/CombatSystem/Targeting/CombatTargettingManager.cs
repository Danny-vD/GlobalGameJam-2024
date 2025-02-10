using System;
using System.Collections.Generic;
using System.Linq;
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
		public event Action OnTargetingStarted = delegate { };

		/// <summary>
		/// Invoked when it is no longer possible to select targets, the boolean is true if the move was cancelled
		/// </summary>
		public event Action<bool> OnTargetingStopped = delegate { };

		public event Action<GameObject> OnTargetAdded = delegate { };
		public event Action<GameObject> OnTargetRemoved = delegate { };

		[SerializeField] //TODO move to a seperate class
		private GameObject currentSelectedCharacterIndicator;

		public bool IsTargetingEnabled { get; private set; }
		
		private readonly List<GameObject> selectedTargets = new List<GameObject>();
		private AbstractCombatMove currentMove;

		private PlayerTurnManager turnManager;

		public static IEnumerable<GameObject> GetAllValidTargets(AbstractCombatMove combatMove, GameObject caster)
		{
			return CombatManager.GetAliveCombatParticipants().Where(participant => combatMove.IsValidTarget(participant, caster));
		}
		
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

		public void EnableTargetingForMove(AbstractCombatMove combatMove, GameObject caster)
		{
			// BUG: left click confirms, but left click also selects a move | trying to select a move after you already selected a move simultaneously confirms the target and then selects the move (which causes you to select a move on someone who is not allowed yet to select a move)

			selectedTargets.Clear();

			currentMove = combatMove;

			IsTargetingEnabled = true;
			OnTargetingStarted.Invoke();
		}

		private void AddTarget(GameObject target)
		{
			if (!target)
			{
				currentSelectedCharacterIndicator.transform.position = Vector3.zero;
			}

			if (!CombatManager.CombatParticipants.Contains(target))
			{
#if UNITY_EDITOR
				Debug.LogError("Tried to target a character that is not in combat!");
#endif

				return;
			}

			selectedTargets.Add(target);

			OnTargetAdded.Invoke(target);
			
			//TODO move to a seperate class
			currentSelectedCharacterIndicator.transform.position = target.transform.position + Vector3.up * 1.5f;
		}

		private void RemoveTarget(GameObject target)
		{
			if (selectedTargets.Remove(target))
			{
				OnTargetRemoved.Invoke(target);
			}
		}

		private void OnCharacterClicked(CharacterClickedEvent @event)
		{
			AddTarget(@event.Character);
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

			IsTargetingEnabled = false;
			OnTargetingStopped.Invoke(false);
		}
	}
}