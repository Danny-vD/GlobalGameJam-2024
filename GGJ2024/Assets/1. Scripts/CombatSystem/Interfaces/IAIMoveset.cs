using CombatMoves.BaseClasses;
using CombatMoves.BaseClasses.ScriptableObjects;

namespace CombatSystem.Interfaces
{
	public interface IAIMoveset : IMoveset
	{
		public AbstractCombatMove ChooseAIMove();
	}
}