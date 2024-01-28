using CombatMoves.BaseClasses;

namespace CombatSystem.Interfaces
{
	public interface IAIMoveset : IMoveset
	{
		public AbstractCombatMove ChooseAIMove();
	}
}