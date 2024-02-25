using CombatMoves.TargetingLogic.Interfaces;
using PlayerPartyScripts;
using UnityEngine;

namespace CombatMoves.TargetingLogic.TargetingValidators
{
    public class TargetOwnTeam : ITargetingValidator
    {
        public bool IsValidTarget(GameObject target, GameObject caster)
        {
            var party = PlayerPartySingleton.Instance.Party;

            return party.Contains(target) == party.Contains(caster);
        }
    }
}