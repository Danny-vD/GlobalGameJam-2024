using System;
using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using UnityEngine;
using VDFramework;

namespace CombatSystem.CharacterScripts
{
	public class ConfirmedMoveHolder : BetterMonoBehaviour
	{
		public event Action OnMoveSelected = delegate { };

		public AbstractCombatMove SelectedMove { get; private set; }
		public List<GameObject> SelectedTargets { get; private set; }

		public void SelectMove(AbstractCombatMove combatMove, GameObject target)
		{
			SelectedMove    = combatMove;
			SelectedTargets = new List<GameObject>() { target };

			OnMoveSelected.Invoke();
		}
		
		public void SelectMove(AbstractCombatMove combatMove, List<GameObject> targets)
		{
			SelectedMove    = combatMove;
			SelectedTargets = targets;

			OnMoveSelected.Invoke();
		}
	}
}