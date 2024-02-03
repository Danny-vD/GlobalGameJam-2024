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

			if (!caster)
			{
				Debug.Log("Caster is null");
			}
			else
			{
				Debug.LogError("Caster: " + caster.name);
			}

			if (!target)
			{
				Debug.Log("Target is null");
			}
			else
			{
				Debug.LogError("Target: " + target.name);
			}

			target.GetComponent<CharacterHealth>().Damage((int)Potency);

			TimerManager.StartNewTimer(1, EndCombatMove);
		}
	}
}