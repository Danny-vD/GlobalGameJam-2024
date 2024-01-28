using System;
using CombatMoves.BaseClasses;
using CombatSystem.Enums;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class CastingState : AbstractCharacterState
	{
		public static Action<Action> OnNewCharacterReadyToCast = delegate { };
		public static Action OnCharacterFinishedCasting = delegate { };
		
		public event Action OnCastingStarted = delegate { };
		public event Action OnCastingEnded = delegate { };
		
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Idle;

		private SelectedMoveHolder selectedMoveHolder;

		private void Awake()
		{
			selectedMoveHolder = GetComponent<SelectedMoveHolder>();
		}

		public override void Enter()
		{
			OnNewCharacterReadyToCast.Invoke(StartCasting);
		}

		private void StartCasting()
		{
			AbstractCombatMove selectedMove = selectedMoveHolder.SelectedMove;
			selectedMove.OnCombatMoveEnded += Exit;
			selectedMove.StartCombatMove(selectedMoveHolder.SelectedTarget);
			
			OnCastingStarted.Invoke();
		}

		public override void Step()
		{
		}

		protected override void Exit()
		{
			selectedMoveHolder.SelectedMove.OnCombatMoveEnded -= Exit;
			
			OnCastingEnded.Invoke();
			base.Exit();
			
			OnCharacterFinishedCasting.Invoke();
		}

		public void EndCast()
		{
			Exit();
		}
	}
}