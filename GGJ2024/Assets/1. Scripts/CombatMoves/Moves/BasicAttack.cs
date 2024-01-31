using CombatMoves.BaseClasses;
using UnityEngine;

namespace CombatMoves.Moves
{
    [CreateAssetMenu(fileName = nameof(BasicAttack), menuName = "CombatMoves/" + nameof(BasicAttack))]
    public class BasicAttack : AbstractCombatMove
    {
        public override void StartCombatMove(GameObject target, GameObject caster)
        {
            throw new System.NotImplementedException();
        }
    }
}