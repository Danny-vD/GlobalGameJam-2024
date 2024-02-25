using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;

namespace CombatSystem.Interfaces
{
    public interface IMoveset
    {
        public List<AbstractCombatMove> GetMoves();
    }
}