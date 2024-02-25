using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Interfaces;
using VDFramework;

namespace CombatSystem.CharacterScripts.MoveSets
{
    public abstract class AbstractMoveset : BetterMonoBehaviour, IMoveset
    {
        public abstract List<AbstractCombatMove> GetMoves();
        public abstract void AddMove(AbstractCombatMove abstractCombatMove);

        public abstract void RemoveMove(AbstractCombatMove abstractCombatMove);
    }
}