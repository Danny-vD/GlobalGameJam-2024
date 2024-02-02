using System.Collections.Generic;
using CombatMoves.BaseClasses;
using CombatMoves.BaseClasses.ScriptableObjects;

namespace CombatSystem.Interfaces
{
	public interface IMoveset
	{
		public List<AbstractCombatMove> GetMoves();
	}
}