using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using UnityEngine;

namespace CombatMoves.ScriptableObjects.Moves
{
	[CreateAssetMenu(fileName = nameof(BasicAttack), menuName = "CombatMoves/" + nameof(BasicAttack))]
	public class Wait : AbstractCombatMove
	{
		public override void StartCombatMove(GameObject target, GameObject caster)
		{
			caster.GetComponent<CharacterStateManager>().RestartCurrentState();
			EndCombatMove();
		}
	}
}