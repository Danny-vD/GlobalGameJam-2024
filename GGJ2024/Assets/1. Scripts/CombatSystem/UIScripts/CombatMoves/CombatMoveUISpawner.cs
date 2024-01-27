using System;
using CombatSystem.CharacterScripts;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Interfaces;
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
			PlayerChoosingState.StartedChoosingState += ShowMoves;
			PlayerChoosingState.EndedChoosingState += HideMoves;
		}

		private void ShowMoves(IMoveset moveset, CombatMoveManager combatMoveManager)
		{
			InstantiateCombatMoves(moveset, combatMoveManager);
			
			OnShowMoves.Invoke();
		}

		private void HideMoves()
		{
			OnHideMoves.Invoke();
		}

		private void InstantiateCombatMoves(IMoveset moveset, CombatMoveManager combatMoveManager)
		{
			combatMovesParent.DestroyChildren();
			
			foreach (CombatMove combatMove in moveset.GetMoves())
			{
				GameObject instance = Instantiate(combatMovePrefab, combatMovesParent);

				instance.GetComponent<CombatMoveUIManager>().Initialize(combatMove, combatMoveManager);
			}
		}
	}
}