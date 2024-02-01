using System.Collections.Generic;
using CombatMoves.BaseClasses;
using UnityEngine;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public class PlayerMoveset : AbstractMoveset
	{
		[SerializeField]
		protected List<AbstractCombatMove> starterMoves;
		
		private void Start()
		{
			foreach (AbstractCombatMove combatMove in starterMoves)
			{
				base.AddMove(combatMove);
			}
		}
	}
}