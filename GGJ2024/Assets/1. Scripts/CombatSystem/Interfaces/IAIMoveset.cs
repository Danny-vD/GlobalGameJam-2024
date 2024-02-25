using CombatMoves.ScriptableObjects.BaseClasses;

namespace CombatSystem.Interfaces
{
    public interface IAIMoveset : IMoveset
    {
        public AbstractCombatMove ChooseAIMove();
    }
}