using System;
using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Enums;
using CombatSystem.Events.Queues;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class CastingState : AbstractCharacterState
	{
		public event Action OnCastingStarted = delegate { };

		public bool IsCasting { get; private set; }

		public override CharacterCombatStateType NextState => CharacterCombatStateType.Idle;

		private ConfirmedMoveHolder confirmedMoveHolder;

		private void Awake()
		{
			confirmedMoveHolder = GetComponent<ConfirmedMoveHolder>();
		}

		public override void Enter()
		{
			EventManager.RaiseEvent(new NewCharacterReadyToCastEvent(this));
		}

		public override void Exit()
		{
			if (IsCasting)
			{
				// This prevents other moves from starting if this was called before the selected move called AllowNextMoveToStart (should never happen)
				
				// no call to base because StopCasting already calls it
				confirmedMoveHolder.SelectedMove.ForceStopCombatMove(gameObject);
			}
			else
			{
				EventManager.RaiseEvent(new CastingPreventedEvent(this));
				base.Exit();
			}
		}

		public void StartCasting()
		{
			AbstractCombatMove selectedMove = confirmedMoveHolder.SelectedMove;

			List<GameObject> validTargets = ValidateTargets();

			if (validTargets.Count == 0)
			{
				base.Exit();
			}

			selectedMove.OnCombatMoveEnded += OnCombatMoveEnded;

			IsCasting = true;
			
			OnCastingStarted.Invoke(); // invoked before the move starts because otherwise it would be invoked after OnStateEnded if the move immediately finishes
			
			selectedMove.StartCombatMove(validTargets, CachedGameObject);
		}

		private void OnCombatMoveEnded(GameObject caster)
		{
			if (ReferenceEquals(caster, gameObject)) // Because the event is shared between all instances, it might be another combat move that ended
			{
				StopCasting();
			}
		}
		
		private void StopCasting()
		{
			confirmedMoveHolder.SelectedMove.OnCombatMoveEnded -= OnCombatMoveEnded;
			IsCasting                                          =  false;
			base.Exit();
		}

		/// <summary>
		/// Removes all invalid targets (e.g. dead ones)
		/// </summary>
		private List<GameObject> ValidateTargets()
		{
			List<GameObject> validTargets = new List<GameObject>(confirmedMoveHolder.SelectedTargets);
			validTargets.RemoveAll(NullOrDead);

			return validTargets;
		}

		private bool NullOrDead(GameObject obj)
		{
			return obj == null || obj.GetComponent<CharacterStateManager>().CurrentStateType == CharacterCombatStateType.Dead;
		}
	}
}