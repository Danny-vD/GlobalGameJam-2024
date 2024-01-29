using System;
using CombatMoves.BaseClasses;
using CombatSystem.Enums;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class CastingState : AbstractCharacterState
	{
		public static Action<CastingState> OnNewCharacterReadyToCast = delegate { };
		public static Action OnCharacterFinishedCasting = delegate { };
		
		public event Action OnCastingStarted = delegate { };
		public event Action OnCastingEnded = delegate { };
		
		public bool IsCasting { get; private set; }
		
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Idle;

		private SelectedMoveHolder selectedMoveHolder;

		private void Awake()
		{
			selectedMoveHolder = GetComponent<SelectedMoveHolder>();
		}

		public override void Enter()
		{
			OnNewCharacterReadyToCast.Invoke(this);
		}

		public override void Step()
		{
		}

		protected override void Exit()
		{
			StopCasting();
		}
		
		public void StartCasting()
		{
			AbstractCombatMove selectedMove = selectedMoveHolder.SelectedMove;
			selectedMove.OnCombatMoveEnded += Exit;
			selectedMove.StartCombatMove(selectedMoveHolder.SelectedTarget);
			
			IsCasting = true;
			OnCastingStarted.Invoke();
		}

		public void StopCasting()
		{
			selectedMoveHolder.SelectedMove.OnCombatMoveEnded -= Exit;
			
			OnCastingEnded.Invoke();
			base.Exit();
			
			IsCasting = false;
			OnCharacterFinishedCasting.Invoke();
		}
	}
}