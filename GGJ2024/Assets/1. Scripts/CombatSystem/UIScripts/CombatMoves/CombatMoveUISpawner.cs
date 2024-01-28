using System;
using CombatMoves.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Interfaces;
using CombatSystem.Managers;
using UnityEngine;
using VDFramework;
using VDFramework.UnityExtensions;

namespace CombatSystem.UIScripts.CombatMoves
{
	public class CombatMoveUISpawner : BetterMonoBehaviour
	{
		public static event Action OnShowMoves = delegate { };
		public static event Action OnHideMoves = delegate { };

		[SerializeField]
		private Transform combatMovesParent; 
		
		[SerializeField]
		private GameObject combatMovePrefab;

		private void Awake()
		{
			PlayerTurnManager.NewCharacterChoosingMove += ShowMoves;
			PlayerTurnManager.OnChoosingQueueEmpty += HideMoves;
		}

		private void ShowMoves(GameObject character)
		{
			IMoveset moveset = character.GetComponent<IMoveset>();
			SelectedMoveHolder selectedMoveHolder = character.GetComponent<SelectedMoveHolder>();
			
			InstantiateCombatMoves(moveset, selectedMoveHolder);
			
			OnShowMoves.Invoke();
		}

		private void HideMoves()
		{
			OnHideMoves.Invoke();
		}

		private void InstantiateCombatMoves(IMoveset moveset, SelectedMoveHolder selectedMoveHolder)
		{
			combatMovesParent.DestroyChildren();
			
			foreach (AbstractCombatMove combatMove in moveset.GetMoves())
			{
				GameObject instance = Instantiate(combatMovePrefab, combatMovesParent);

				instance.GetComponent<CombatMoveUIManager>().Initialize(combatMove, selectedMoveHolder);
			}
		}
	}
}