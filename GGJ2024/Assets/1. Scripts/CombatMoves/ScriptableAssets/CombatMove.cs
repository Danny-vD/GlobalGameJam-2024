using CombatMoves.TargetingLogic.Interfaces;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatMoves.ScriptableAssets
{
	[CreateAssetMenu(fileName = "Combatmove", menuName = "Combat/Move")]
	public class CombatMove : ScriptableObject
	{
		public string Name;
		public string Description;
		
		public int Cost = 0;

		public DamageType DamageType = DamageType.Normal;

		public float Potency = 10;

		public ITargetingValidator TargetingValidator;
	}
}