using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatMoves.ScriptableObjects.Moves
{
	[CreateAssetMenu(fileName = nameof(Wait), menuName = "CombatMoves/" + nameof(Wait))]
	public class Wait : AbstractCombatMove
	{
		public override void StartCombatMove(GameObject target, GameObject caster)
		{
			// TODO: this causes an infinite loop for AI when this is the only move because choosing state immediately exists into casting
			caster.GetComponent<CharacterStateManager>().ForceState(CharacterCombatStateType.Idle);
			EndCombatMove();
		}
	}
}