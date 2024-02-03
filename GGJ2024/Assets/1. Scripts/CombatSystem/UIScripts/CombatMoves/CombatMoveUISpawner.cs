using System;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Events.Queues;
using CombatSystem.Interfaces;
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
			//TODO React to player death and stun
			NewPlayerChoosingMoveEvent.Listeners            += ShowMoves;
			AllPlayersChoseMoveEvent.ParameterlessListeners += HideMoves;
		}

		private void OnDestroy()
		{
			NewPlayerChoosingMoveEvent.Listeners            -= ShowMoves;
			AllPlayersChoseMoveEvent.ParameterlessListeners -= HideMoves;
		}

		private void ShowMoves(NewPlayerChoosingMoveEvent newPlayerChoosingMoveEvent)
		{
			GameObject player = newPlayerChoosingMoveEvent.Player;

			IMoveset moveset = player.GetComponent<IMoveset>();

			InstantiateCombatMoves(moveset, player);

			OnShowMoves.Invoke();
		}

		private void HideMoves()
		{
			OnHideMoves.Invoke();
		}

		private void InstantiateCombatMoves(IMoveset moveset, GameObject player)
		{
			combatMovesParent.DestroyChildren();

			foreach (AbstractCombatMove combatMove in moveset.GetMoves())
			{
				GameObject instance = Instantiate(combatMovePrefab, combatMovesParent);

				instance.GetComponent<CombatMoveUIManager>().Initialize(combatMove, player);
			}
		}
	}
}