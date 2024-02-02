using System.Collections.Generic;
using CombatMoves.BaseClasses;
using CombatMoves.BaseClasses.ScriptableObjects;
using CombatSystem.Interfaces;
using VDFramework;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public abstract class AbstractMoveset : BetterMonoBehaviour, IMoveset
	{
		public abstract void AddMove(AbstractCombatMove abstractCombatMove);

		public abstract void RemoveMove(AbstractCombatMove abstractCombatMove);

		public abstract List<AbstractCombatMove> GetMoves();
	}
}