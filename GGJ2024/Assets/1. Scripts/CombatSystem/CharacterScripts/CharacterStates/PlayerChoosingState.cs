using System;
using CombatSystem.Enums;
using CombatSystem.Interfaces;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(CombatMoveManager))]
	public class PlayerChoosingState : AbstractCharacterState
	{
		public static event Action<IMoveset, CombatMoveManager> StartedChoosingState = delegate { };
		public static event Action EndedChoosingState = delegate { };

		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;
		
		private CombatMoveManager combatMoveManager;
		private IMoveset moveset;

		private void Awake()
		{
			combatMoveManager                =  GetComponent<CombatMoveManager>();
			moveset                          =  GetComponent<IMoveset>();
			
			combatMoveManager.OnMoveSelected += Exit;
		}
		
		public override void Enter()
		{
			StartedChoosingState.Invoke(moveset, combatMoveManager);
		}

		public override void Step()
		{
		}

		protected override void Exit()
		{
			base.Exit();
			
			EndedChoosingState.Invoke();
		}

		private void OnDestroy()
		{
			combatMoveManager.OnMoveSelected -= Exit;
		}
	}
}