using CombatMoves.TargetingLogic.Interfaces;
using UnityEngine;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
    public class TargetCombined : ITargetingValidator
    {
        private readonly ITargetingValidator[] targetingValidators;

        public TargetCombined(params ITargetingValidator[] validators)
        {
            targetingValidators = validators;
        }

        public bool IsValidTarget(GameObject target, GameObject caster)
        {
            for (var i = 0; i < targetingValidators.Length; i++)
                if (!targetingValidators[i].IsValidTarget(target, caster))
                    return false;

            return true;
        }
    }
}