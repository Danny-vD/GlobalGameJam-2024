using System;
using CombatSystem.Events.Queues;
using CombatSystem.Interfaces;
using UnityEngine;
using VDFramework;
using VDFramework.UnityExtensions;

namespace CombatSystem.UIScripts.CombatMoves
{
    public class CombatMoveUISpawner : BetterMonoBehaviour
    {
        [SerializeField] private Transform combatMovesParent;

        [SerializeField] private GameObject combatMovePrefab;

        private void Awake()
        {
            NewPlayerChoosingMoveEvent.Listeners += ShowMoves;
            AllPlayersChoseMoveEvent.ParameterlessListeners += HideMoves;
        }

        private void OnDestroy()
        {
            NewPlayerChoosingMoveEvent.Listeners -= ShowMoves;
            AllPlayersChoseMoveEvent.ParameterlessListeners -= HideMoves;
        }

        public static event Action OnShowMoves = delegate { };
        public static event Action OnHideMoves = delegate { };

        private void ShowMoves(NewPlayerChoosingMoveEvent newPlayerChoosingMoveEvent)
        {
            var player = newPlayerChoosingMoveEvent.Player;

            var moveset = player.GetComponent<IMoveset>();

            InstantiateCombatMoves(moveset, player);

            OnShowMoves.Invoke();
        }

        public static void HideMoves()
        {
            OnHideMoves.Invoke();
        }

        private void InstantiateCombatMoves(IMoveset moveset, GameObject player)
        {
            combatMovesParent.DestroyChildren();

            foreach (var combatMove in moveset.GetMoves())
            {
                var instance = Instantiate(combatMovePrefab, combatMovesParent);

                instance.GetComponent<CombatMoveUIManager>().Initialize(combatMove, player);
            }
        }
    }
}