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

		protected override void Exit()
		{
			StopCasting();
		}

		public void StartCasting()
		{
			//TODO: Check whether the target is still valid (might be dead)
			AbstractCombatMove selectedMove = confirmedMoveHolder.SelectedMove;

			selectedMove.OnCombatMoveEnded += Exit;

			selectedMove.StartCombatMove(confirmedMoveHolder.SelectedTarget, CachedGameObject);

			IsCasting = true;
			OnCastingStarted.Invoke();
		}

		public void StopCasting()
		{
			confirmedMoveHolder.SelectedMove.OnCombatMoveEnded -= Exit;

			IsCasting = false;
			base.Exit();
		}
	}
}