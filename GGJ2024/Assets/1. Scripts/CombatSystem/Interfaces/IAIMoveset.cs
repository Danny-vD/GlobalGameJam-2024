using CombatSystem.ScriptableAssets.CombatMoves;

namespace CombatSystem.Interfaces
{
	public interface IAIMoveset : IMoveset
	{
		public CombatMove ChooseAIMove();
	}
}