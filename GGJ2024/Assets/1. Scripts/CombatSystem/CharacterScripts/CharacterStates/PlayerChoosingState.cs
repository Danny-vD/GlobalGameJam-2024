using CombatSystem.Enums;
using CombatSystem.Events.CharacterStateEvents;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(ConfirmedMoveHolder))]
	public class PlayerChoosingState : AbstractCharacterState
	{
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

		private ConfirmedMoveHolder confirmedMoveHolder;

		private void Awake()
		{
			confirmedMoveHolder = GetComponent<ConfirmedMoveHolder>();

			confirmedMoveHolder.OnMoveSelected += Exit;
		}
		
		public override void Enter()
		{
			EventManager.RaiseEvent(new PlayerEnteredChoosingStateEvent(transform.root.gameObject));
		}

		public override void Exit()
		{
			EventManager.RaiseEvent(new PlayerExitedChoosingStateEvent(transform.root.gameObject));
			base.Exit();
		}

		private void OnDestroy()
		{
			confirmedMoveHolder.OnMoveSelected -= Exit;
		}
	}
}