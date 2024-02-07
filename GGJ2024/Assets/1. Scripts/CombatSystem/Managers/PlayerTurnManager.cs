using System.Collections.Generic;
using CombatSystem.Events;
using CombatSystem.Events.CharacterStateEvents;
using CombatSystem.Events.Queues;
using UnityEditor;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Managers
{
	public class PlayerTurnManager : BetterMonoBehaviour
	{
		// A list that acts like a queue, we use a list to be able to remove from any position (in case of dead/stunned)
		private readonly List<GameObject> playerTurnQueue = new List<GameObject>(6);

		private GameObject currentlyActivePlayer;

		private void OnEnable()
		{
			PlayerEnteredChoosingStateEvent.Listeners += AddToQueue;
			PlayerExitedChoosingStateEvent.Listeners  += RemoveFromQueue;
			CombatStartedEvent.ParameterlessListeners += ResetQueue;
			CombatEndedEvent.ParameterlessListeners   += ResetQueue;
		}

		private void OnDisable()
		{
			PlayerEnteredChoosingStateEvent.Listeners -= AddToQueue;
			PlayerExitedChoosingStateEvent.Listeners  -= RemoveFromQueue;
			CombatStartedEvent.ParameterlessListeners -= ResetQueue;
			CombatEndedEvent.ParameterlessListeners   -= ResetQueue;
		}

		[MenuItem("Combat/Start Combat &g")] //TODO: remove
		private static void DebugStartCombat()
		{
			EventManager.RaiseEvent(new CombatStartedEvent(null));
		}

		[MenuItem("Combat/Stop Combat")] //TODO: remove
		private static void DebugEndCombat()
		{
			EventManager.RaiseEvent(new CombatEndedEvent());
		}

		public bool TryGetActivePlayer(out GameObject activePlayer)
		{
			activePlayer = currentlyActivePlayer;

			return currentlyActivePlayer != null;
		}
		
		private void ResetQueue()
		{
			playerTurnQueue.Clear();
		}

		private void AddToQueue(PlayerEnteredChoosingStateEvent enteredChoosingStateEvent)
		{
			GameObject player = enteredChoosingStateEvent.Player;

			AddToQueue(player);
		}
		
		private void RemoveFromQueue(PlayerExitedChoosingStateEvent exitedChoosingStateEvent)
		{
			RemoveFromQueue(exitedChoosingStateEvent.Player);
		}

		private void AddToQueue(GameObject player)
		{
			if (playerTurnQueue.Contains(player))
			{
				Debug.LogError("The queue already contains this character!\n" + player.name);
				return;
			}

			playerTurnQueue.Add(player);

			if (playerTurnQueue.Count == 1)
			{
				// First character added so it is always going to be the one that is choosing a move
				SetNewActivePlayer(player);
			}
		}

		private void RemoveFromQueue(GameObject player)
		{
			if (playerTurnQueue.Contains(player))
			{
				if (ReferenceEquals(player, currentlyActivePlayer))
				{
					SetNextInQueueActive(); // SetNextInQueueActive removes element 0 from the list, which is the active player
				}
				else
				{
					playerTurnQueue.Remove(player);
				}
			}
			else
			{
				Debug.LogError($"Attempting to dequeue an gameobject that is not in line!\ndequeuing: {player.name}");
			}
		}

		private void SetNextInQueueActive()
		{
			playerTurnQueue.RemoveAt(0); // Dequeue the first player

			if (playerTurnQueue.Count == 0) // Queue is empty
			{
				SetNoActivePlayer();
			}
			else
			{
				GameObject next = playerTurnQueue[0];
				
				SetNewActivePlayer(next);
			}
		}

		private void SetNewActivePlayer(GameObject player)
		{
			currentlyActivePlayer = player;

			EventManager.RaiseEvent(new NewPlayerChoosingMoveEvent(player));
		}

		private void SetNoActivePlayer()
		{
			currentlyActivePlayer = null;

			EventManager.RaiseEvent(new AllPlayersChoseMoveEvent());
		}
	}
}