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
        public override void StartCombatMove(GameObject target, GameObject caster)
        {
            AudioPlayer.PlayOneShot2D(AudioEventType.SFX_Battle_HitEnemy);

            TimerManager.StartNewTimer(1, InvokeCombatMoveEnded);
        }
    }
}