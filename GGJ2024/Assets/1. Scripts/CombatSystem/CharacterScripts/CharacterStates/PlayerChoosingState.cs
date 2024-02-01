using System;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(SelectedMoveHolder))]
	public class PlayerChoosingState : AbstractCharacterState
	{
		public static event Action<GameObject> StartedChoosingState = delegate { };
		public static event Action<GameObject> EndedChoosingState = delegate { };

		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

		private SelectedMoveHolder selectedMoveHolder;

		private void Awake()
		{
			selectedMoveHolder = GetComponent<SelectedMoveHolder>();

			selectedMoveHolder.OnMoveSelected += Exit;
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
			selectedMoveHolder.OnMoveSelected -= Exit;
		}
	}
}