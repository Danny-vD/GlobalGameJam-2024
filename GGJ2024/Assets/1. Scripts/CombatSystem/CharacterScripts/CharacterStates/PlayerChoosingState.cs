using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(CombatMoveManager))]
	public class PlayerChoosingState : AbstractCharacterState
	{
		public override CharacterStateType NextState => CharacterStateType.Casting;
		
		private CombatMoveManager combatMoveManager;

		private void Awake()
		{
			combatMoveManager = GetComponent<CombatMoveManager>();
			combatMoveManager.OnMoveSelected += Exit;
		}
		
		public override void Enter()
		{
			//TODO show UI
		}

		public override void Step()
		{
		}

		protected override void Exit()
		{
			base.Exit();
			
			//TODO hide UI
		}
	}
}