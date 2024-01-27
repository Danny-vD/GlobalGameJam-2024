using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem
{
	[CreateAssetMenu(fileName = "Combatmove", menuName = "Combat/Move")]
	public class CombatMove : ScriptableObject
	{
		public string Name;
		public string Description;
		
		public int Cost = 0;
		public float CastingTime = 1;

		public DamageType DamageType = DamageType.Normal;

		public float Potency = 10;
	}
}