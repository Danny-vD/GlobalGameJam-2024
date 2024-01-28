using System;
using CombatSystem.CharacterScripts;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Interfaces;
using CombatSystem.Managers;
using CombatSystem.ScriptableAssets.CombatMoves;
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
			//TODO: ignore if someone is already choosing a move
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
			
			foreach (CombatMove combatMove in moveset.GetMoves())
			{
				GameObject instance = Instantiate(combatMovePrefab, combatMovesParent);

				instance.GetComponent<CombatMoveUIManager>().Initialize(combatMove, selectedMoveHolder);
			}
		}
	}
}