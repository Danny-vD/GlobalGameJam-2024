using CombatSystem.Structs;
using UnityEngine;
using VDFramework;

namespace CharacterScripts
{
	public class Character : BetterMonoBehaviour
	{
		[field: SerializeField]
		public CharacterStatistics Statistics { get; private set; }
	}
}