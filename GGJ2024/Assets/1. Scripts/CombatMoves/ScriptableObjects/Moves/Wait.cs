using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatMoves.ScriptableObjects.Moves
{
	[CreateAssetMenu(fileName = nameof(BasicAttack), menuName = "CombatMoves/" + nameof(BasicAttack))]
	public class Wait : AbstractCombatMove
	{
		public override void StartCombatMove(GameObject target, GameObject caster)
		{
			caster.GetComponent<CharacterStateManager>().ForceState(CharacterCombatStateType.Choosing);
			EndCombatMove();
		}
	}
}