using System.Collections.Generic;
using CombatSystem.ScriptableAssets.CombatMoves;

namespace CombatSystem.Interfaces
{
	public interface IMoveset
	{
		public List<CombatMove> GetMoves();
	}
}