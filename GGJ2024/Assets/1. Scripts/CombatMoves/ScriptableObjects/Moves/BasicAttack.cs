using System;
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
        [SerializeField]
        private AudioEventType audioType;
        
        public override void StartCombatMove(GameObject target, GameObject caster)
        {
            AudioPlayer.PlayOneShot2D(audioType);
            
            try
            {
                target.GetComponent<CharacterHealth>().Damage((int)Potency);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log(caster.name);
                Debug.Log(target.name);
            }
            
            TimerManager.StartNewTimer(1, EndCombatMove);
        }
    }
}