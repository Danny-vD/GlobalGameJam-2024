using System;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(CombatMoveManager))]
	public class PlayerChoosingState : AbstractCharacterState
	{
		public static event Action<GameObject> StartedChoosingState = delegate { };
		public static event Action<GameObject> EndedChoosingState = delegate { };

		public override CharacterStateType NextState => CharacterStateType.Casting;
		
		private CombatMoveManager combatMoveManager;

		private void Awake()
		{
			combatMoveManager = GetComponent<CombatMoveManager>();
			combatMoveManager.OnMoveSelected += Exit;
		}
		
		public override void Enter()
		{
			StartedChoosingState.Invoke(gameObject);
		}

		public override void Step()
		{
		}

		protected override void Exit()
		{
			base.Exit();
			
			EndedChoosingState.Invoke(gameObject);
		}

		private void OnDestroy()
		{
			combatMoveManager.OnMoveSelected -= Exit;
		}
	}
}