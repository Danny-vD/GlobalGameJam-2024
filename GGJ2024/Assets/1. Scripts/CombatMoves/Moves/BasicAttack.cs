using CombatMoves.BaseClasses;
using FMODUtilityPackage.Core;
using FMODUtilityPackage.Enums;
using UnityEngine;
using VDFramework.Utility.TimerUtil;

namespace CombatMoves.Moves
{
    [CreateAssetMenu(fileName = nameof(BasicAttack), menuName = "CombatMoves/" + nameof(BasicAttack))]
    public class BasicAttack : AbstractCombatMove
    {
        [SerializeField]
        private AudioEventType audioType;
        
        public override void StartCombatMove(GameObject target, GameObject caster)
        {
            AudioPlayer.PlayOneShot2D(audioType);

            TimerManager.StartNewTimer(1, InvokeCombatMoveEnded);
        }
    }
}