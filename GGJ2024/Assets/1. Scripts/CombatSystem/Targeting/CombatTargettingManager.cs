using System;
using System.Collections.Generic;
using System.Linq;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatMoves.TargetingLogic.Enums;
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

		public event Action OnTargetsValid = delegate { };
		public event Action OnTargetsInvalid = delegate { };

		[SerializeField] //TODO move to a seperate class
		private GameObject currentSelectedCharacterIndicator;

		public bool IsTargetingEnabled { get; private set; }
		public bool HasValidTargets { get; private set; }

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
		
		private void EnableTargeting()
		{
			IsTargetingEnabled = true;
			OnTargetingStarted.Invoke();
			
			EventManager.AddListener<CharacterClickedEvent>(OnCharacterClicked);
		}

		private void DisableTargeting(bool cancelledMove)
		{
			IsTargetingEnabled = false;
			OnTargetingStopped.Invoke(cancelledMove);
		}

		public void EnableTargetingForMove(AbstractCombatMove combatMove, GameObject caster)
		{
			// BUG: left click confirms, but left click also selects a move | trying to select a move after you already selected a move simultaneously confirms the target and then selects the move (which causes you to select a move on someone who is not allowed yet to select a move)

			if (IsTargetingEnabled)
			{
				DisableTargeting(true);
			}
			
			selectedTargets.Clear();
			SetHasValidTargetsFlag(false);

			currentMove = combatMove;
			
			switch (currentMove.TargetingMode)
			{
				case TargetingMode.TargetAll:
					AddTargets(GetAllValidTargets(combatMove, caster));
					break;
				case TargetingMode.SingleTargetOnly:
				case TargetingMode.MultipleTargetsOnly:
				case TargetingMode.MultipleTargets:
					EnableTargeting();
					break;
				case TargetingMode.None:
					CheckValidTargets();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void AddTargets(IEnumerable<GameObject> targets)
		{
			foreach (GameObject target in targets)
			{
				AddTarget(target);
			}
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
			CheckValidTargets();
			
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

		private void CheckValidTargets()
		{
			switch (currentMove.TargetingMode)
			{
				case TargetingMode.TargetAll: // We auto-targetted all valid targets | Alternatively check selected targets against all valid targets
					SetHasValidTargetsFlag(true);
					break;
				case TargetingMode.SingleTargetOnly:
					SetHasValidTargetsFlag(selectedTargets.Count == 1);
					break;
				case TargetingMode.MultipleTargets:
					SetHasValidTargetsFlag(selectedTargets.Count > 1);
					break;
				case TargetingMode.MultipleTargetsOnly:
					SetHasValidTargetsFlag(selectedTargets.Count >= 1);
					break;
				case TargetingMode.None:
					SetHasValidTargetsFlag(selectedTargets.Count == 0);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void SetHasValidTargetsFlag(bool hasValidTargets)
		{
			if (hasValidTargets && !HasValidTargets)
			{
				HasValidTargets = true;
				OnTargetsValid.Invoke();
			}
			else if (!hasValidTargets && HasValidTargets)
			{
				HasValidTargets = false;
				OnTargetsInvalid.Invoke();
			}
		}

		private void OnCharacterClicked(CharacterClickedEvent @event)
		{
			AddTarget(@event.Character);
		}

		public void OnTargetSelectConfirm()
		{
			if (!HasValidTargets)
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

			DisableTargeting(false);
		}
	}
}