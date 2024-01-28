using System;
using CombatMoves.BaseClasses;
using VDFramework;

namespace CombatSystem.CharacterScripts
{
	public class SelectedMoveHolder : BetterMonoBehaviour
	{
		public event Action OnMoveSelected = delegate { };
		
		public AbstractCombatMove SelectedMove { get; private set; }

		public void SelectMove(AbstractCombatMove moveData)
		{
			SelectedMove = moveData;
			
			OnMoveSelected.Invoke();
		}
	}
}