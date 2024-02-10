using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatMoves.ScriptableObjects.Moves
{
	[CreateAssetMenu(fileName = nameof(Wait), menuName = "CombatMoves/" + nameof(Wait))]
	public class Wait : AbstractCombatMove
	{
		public override void StartCombatMove(List<GameObject> targets, GameObject caster)
		{
			AllowNextMoveToStart(); // Allow the next move to start to properly clean up the queue then force exit into choosing state (which also forces this combat move to end)
			
			// TODO: this causes an infinite loop for AI when this is the only move because choosing state immediately exists into casting
			caster.GetComponent<CharacterStateManager>().ForceState(CharacterCombatStateType.Idle); // HACK use state.Choosing instead
		}
	}
}