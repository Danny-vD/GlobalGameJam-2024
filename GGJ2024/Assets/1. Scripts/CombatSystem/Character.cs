using CombatSystem.Structs;
using UnityEngine;
using VDFramework;

namespace CombatSystem
{
	public class Character : BetterMonoBehaviour
	{
		[field: SerializeField]
		public CharacterStatistics Statistics { get; private set; }
	}
}