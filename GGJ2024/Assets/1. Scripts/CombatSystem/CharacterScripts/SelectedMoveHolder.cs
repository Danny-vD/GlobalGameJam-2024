using System;
using CombatMoves.ScriptableObjects.BaseClasses;
using UnityEngine;
using VDFramework;

namespace CombatSystem.CharacterScripts
{
	public class SelectedMoveHolder : BetterMonoBehaviour
	{
		public event Action OnMoveSelected = delegate { };
		
		public AbstractCombatMove SelectedMove { get; private set; }
		public GameObject SelectedTarget { get; private set; }

		public void SelectMove(AbstractCombatMove combatMove, GameObject target)
		{
			SelectedMove   = combatMove;
			SelectedTarget = target;
			
			OnMoveSelected.Invoke();
		}
	}
}