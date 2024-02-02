using System.Collections.Generic;
using CombatMoves.BaseClasses;
using CombatMoves.BaseClasses.ScriptableObjects;
using UnityEngine;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public class PlayerMoveset : AbstractMoveset
	{
		[SerializeField]
		protected List<AbstractCombatMove> moves;

		public override void AddMove(AbstractCombatMove abstractCombatMove)
		{
			if (moves.Contains(abstractCombatMove))
			{
				return;
			}
			
			moves.Add(abstractCombatMove);
		}

		public override void RemoveMove(AbstractCombatMove abstractCombatMove)
		{
			moves.Remove(abstractCombatMove);
		}

		public override List<AbstractCombatMove> GetMoves()
		{
			return moves;
		}
	}
}