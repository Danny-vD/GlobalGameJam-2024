using System;
using CombatSystem.ScriptableAssets.CombatMoves;
using VDFramework;

namespace CombatSystem.CharacterScripts
{
	public class CombatMoveManager : BetterMonoBehaviour
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