using System;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Enums;
using CombatSystem.Events.Queues;
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
				confirmedMoveHolder.SelectedMove.ForceStopCombatMove();
			}
			else
			{
				EventManager.RaiseEvent(new CastingPreventedEvent(this));
				base.Exit();
			}
		}

		public void StartCasting()
		{
			//TODO: Check whether the target is still valid (might be dead)
			AbstractCombatMove selectedMove = confirmedMoveHolder.SelectedMove;
			
			if (confirmedMoveHolder.SelectedTarget.GetComponent<CharacterStateManager>().CurrentStateType == CharacterCombatStateType.Dead) // HACK refactor this
			{
				base.Exit();
			}

			selectedMove.OnCombatMoveEnded += StopCasting;

			selectedMove.StartCombatMove(confirmedMoveHolder.SelectedTarget, CachedGameObject);

			IsCasting = true;

			OnCastingStarted.Invoke();
		}

		public void StopCasting()
		{
			confirmedMoveHolder.SelectedMove.OnCombatMoveEnded -= StopCasting;

			IsCasting = false;
			base.Exit();
		}
	}
}