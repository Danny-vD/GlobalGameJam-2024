using System.Collections.Generic;
using CombatMoves.BaseClasses;
using CombatSystem.Interfaces;
using VDFramework;

namespace CombatSystem.CharacterScripts.MoveSets
{
	public abstract class AbstractMoveset : BetterMonoBehaviour, IMoveset
	{
		protected List<AbstractCombatMove> currentMoves = new List<AbstractCombatMove>();

		public virtual T AddMove<T>(T newMove) where T : AbstractCombatMove
		{
			T component = gameObject.AddComponent(newMove.GetType()) as T;
			currentMoves.Add(component);

			return component;
		}
		
		public virtual T AddMove<T>() where T : AbstractCombatMove
		{
			T combatMove = gameObject.AddComponent<T>();
			currentMoves.Add(combatMove);

			return combatMove;
		}

		public virtual void RemoveMove<T>() where T : AbstractCombatMove
		{
			AbstractCombatMove combatMove = GetComponent<T>();

			if (combatMove == null)
			{
				return;
			}
			
			OnRemoveMove(combatMove);

			currentMoves.Remove(combatMove);

			Destroy(combatMove);
		}

		protected virtual void OnRemoveMove<T>(T combatMove) where T : AbstractCombatMove
		{
		}

		public virtual List<AbstractCombatMove> GetMoves()
		{
			return new List<AbstractCombatMove>(currentMoves);
		}
	}
}