using UnityEngine;

namespace CombatMoves.TargetingLogic.Interfaces
{
    public interface ITargetingValidator
    {
        public bool IsValidTarget(GameObject target, GameObject caster);
    }
}