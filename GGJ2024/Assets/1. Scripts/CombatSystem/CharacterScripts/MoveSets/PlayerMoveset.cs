using System.Collections.Generic;
using CombatSystem.Interfaces;
using UnityEngine;
using VDFramework;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public class PlayerMoveset : BetterMonoBehaviour, IMoveset
	{
		[SerializeField]
		private List<CombatMove> moves;

		public List<CombatMove> GetMoves()
		{
			return new List<CombatMove>(moves);
		}
	}
}