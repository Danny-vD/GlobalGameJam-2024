using System;
using CombatMoves.ScriptableAssets;
using VDFramework;

namespace CombatSystem.CharacterScripts
{
	public class SelectedMoveHolder : BetterMonoBehaviour
	{
		public event Action OnMoveSelected = delegate { };
		
		public CombatMove SelectedMove { get; private set; }

		public void SelectMove(CombatMove move)
		{
			SelectedMove = move;
			
			OnMoveSelected.Invoke();
		}
	}
}