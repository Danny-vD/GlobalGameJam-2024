using System;
using CombatSystem.Enums;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class CastingState : AbstractCharacterState
	{
		public event Action OnCastingStarted = delegate { };
		public event Action OnCastingEnded = delegate { };
		
		public override CharacterStateType NextState => CharacterStateType.Idle;

		public override void Enter()
		{
			OnCastingStarted.Invoke();
		}

		public override void Step()
		{
		}

		protected override void Exit()
		{
			OnCastingEnded.Invoke();
			base.Exit();
		}

		public void EndCast()
		{
			Exit();
		}
	}
}