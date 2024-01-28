using System.Collections.Generic;
using CombatMoves.ScriptableAssets;

namespace CombatSystem.Interfaces
{
	public interface IMoveset
	{
		public List<CombatMove> GetMoves();
	}
}