using System.Collections.Generic;
using System.Linq;
using CharacterScripts;
using CombatMoves.ScriptableObjects.BaseClasses;
using PlayerPartyScripts;
using UnityEngine;
using VDFramework;

namespace AI
{
    [DisallowMultipleComponent]
    public abstract class AbstractAITargetingLogic : BetterMonoBehaviour
    {
        public abstract List<GameObject> GetTargets(AbstractCombatMove combatMove);

        public IEnumerable<GameObject> GetAllValidTargets(AbstractCombatMove combatMove)
        {
            return GetCombatParticipants().Where(participant => combatMove.IsValidTarget(participant, gameObject));
        }

        protected IEnumerable<GameObject> GetCombatParticipants()
        {
            //TODO properly get all participants in combat (combat manager?)
            var party = PlayerPartySingleton.Instance.Party.Where(obj => !obj.GetComponent<CharacterHealth>().IsDead);

            return party;
        }
    }
}