using System.Collections.Generic;
using System.Linq;
using CharacterScripts;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Managers;
using PlayerPartyScripts;
using UnityEngine;
using VDFramework;

namespace AI
{
    [DisallowMultipleComponent]
    public abstract class AbstractAITargetingLogic : BetterMonoBehaviour
    {
        public abstract List<GameObject> GetTargets(AbstractCombatMove combatMove);
    }
}