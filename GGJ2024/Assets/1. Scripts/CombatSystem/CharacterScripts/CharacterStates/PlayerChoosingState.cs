using CombatSystem.Enums;
using CombatSystem.Events.CharacterStateEvents;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(SelectedMoveHolder))]
	public class PlayerChoosingState : AbstractCharacterState
	{
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

		private SelectedMoveHolder selectedMoveHolder;

		private void Awake()
		{
			selectedMoveHolder = GetComponent<SelectedMoveHolder>();

			selectedMoveHolder.OnMoveSelected += Exit;
		}

        // TODO: override target selection when taunted?? (Can a player be taunted?)
		public override void Enter()
		{
			EventManager.RaiseEvent(new PlayerEnteredChoosingStateEvent(gameObject));
		}

		protected override void Exit()
		{
			EventManager.RaiseEvent(new PlayerExitedChoosingStateEvent(gameObject));
			base.Exit();
		}

		private void OnDestroy()
		{
			selectedMoveHolder.OnMoveSelected -= Exit;
		}
	}
}