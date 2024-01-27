namespace CombatSystem.Interfaces
{
	public interface IAIMoveset : IMoveset
	{
		public CombatMove ChooseAIMove();
	}
}