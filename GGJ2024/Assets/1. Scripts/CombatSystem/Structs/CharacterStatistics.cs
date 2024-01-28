using System;

namespace CombatSystem.Structs
{
	[Serializable]
	public struct CharacterStatistics
	{
		public string Name;
		
		public float Speed;

		public float AttackPower;
		public float SpecialAttackPower;

		public float Defense;
		public float SpecialDefense;
	}
}