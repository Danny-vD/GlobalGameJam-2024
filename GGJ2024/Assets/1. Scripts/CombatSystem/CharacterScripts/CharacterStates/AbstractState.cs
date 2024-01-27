using System;
using CombatSystem.Enums;
using VDFramework;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public abstract class AbstractCharacterState : BetterMonoBehaviour
	{
		public event Action<CharacterStateType> OnStateEnded = delegate { };

		public abstract CharacterStateType NextState { get; }
		
		public abstract void Enter();

		public abstract void Step();

		protected virtual void Exit()
		{
			OnStateEnded.Invoke(NextState);
		}
	}
}