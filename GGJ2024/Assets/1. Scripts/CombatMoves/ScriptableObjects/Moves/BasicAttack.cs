using System.Collections.Generic;
using CharacterScripts;
using CombatMoves.ScriptableObjects.BaseClasses;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using UnityEngine;
using VDFramework.Utility.TimerUtil;

namespace CombatMoves.ScriptableObjects.Moves
{
    [CreateAssetMenu(fileName = nameof(BasicAttack), menuName = "CombatMoves/" + nameof(BasicAttack))]
    public class BasicAttack : AbstractCombatMove
    {
        [SerializeField] private AudioEventType audioType;

        public override void StartCombatMove(List<GameObject> targets, GameObject caster)
        {
            AudioPlayer.PlayOneShot2D(audioType);
            
            // TODO: Call this method when the animation occurs instead
            TimerManager.StartNewTimer(1, EndCombatMove, false, caster);
            
            foreach (var target in targets) target.GetComponent<CharacterHealth>().Damage((int)Potency);
        }
    }
}