using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Targeting;
using UnityEngine;
using VDFramework.Extensions;

namespace AI
{
    public class RandomTargetLogic : AbstractAITargetingLogic
    {
        public override List<GameObject> GetTargets(AbstractCombatMove combatMove)
        {
            List<GameObject> targets = new List<GameObject> { GetRandomTarget(combatMove) };
            return targets;
        }

        private GameObject GetRandomTarget(AbstractCombatMove combatMove)
        {
            return CombatTargetingManager.GetAllValidTargets(combatMove, gameObject).GetRandomElement();
        }
    }
}