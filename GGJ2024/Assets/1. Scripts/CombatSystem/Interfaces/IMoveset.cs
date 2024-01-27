using System.Collections.Generic;

namespace CombatSystem.Interfaces
{
	public interface IMoveset
	{
		public List<CombatMove> GetMoves();
	}
}