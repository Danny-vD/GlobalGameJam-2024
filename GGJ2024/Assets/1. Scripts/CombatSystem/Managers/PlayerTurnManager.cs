using System.Collections.Generic;
using CharacterScripts;
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
		private readonly Queue<GameObject> characterPickingMoveQueue = new Queue<GameObject>();

		private GameObject currentlyActivePlayer;

		private void OnEnable()
		{
			PlayerEnteredChoosingStateEvent.Listeners += AddToQueue;
			PlayerExitedChoosingStateEvent.Listeners  += RemoveFromQueue;
			CombatStartedEvent.ParameterlessListeners += ResetState;
			CombatEndedEvent.ParameterlessListeners   += ResetState;
		}

		private void OnDisable()
		{
			PlayerEnteredChoosingStateEvent.Listeners -= AddToQueue;
			PlayerExitedChoosingStateEvent.Listeners  -= RemoveFromQueue;
			CombatStartedEvent.ParameterlessListeners -= ResetState;
			CombatEndedEvent.ParameterlessListeners   -= ResetState;
		}

		private void ResetState()
		{
			characterPickingMoveQueue.Clear();
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

		private void AddToQueue(PlayerEnteredChoosingStateEvent enteredChoosingStateEvent)
		{
			GameObject player = enteredChoosingStateEvent.Player;

			if (characterPickingMoveQueue.Contains(player))
			{
				Debug.LogError("The queue already contains this character!\n" + player.name);
				return;
			}

			characterPickingMoveQueue.Enqueue(player);

			if (characterPickingMoveQueue.Count == 1)
			{
				// First character added so it is always going to be the one that is choosing a move
				SetNewActivePlayer(player);
			}
		}

		private void RemoveFromQueue(PlayerExitedChoosingStateEvent exitedChoosingStateEvent)
		{
			if (characterPickingMoveQueue.TryPeek(out GameObject next))
			{
				GameObject player = exitedChoosingStateEvent.Player;

				if (ReferenceEquals(player, next))
				{
					SetNextInQueueActive();

					return;
				}

				Debug.LogError($"Attempting to dequeue the gameobject that is not next in line!\ndequeuing: {player.name}\tNext: {next.name}");
				return;
			}

			Debug.LogError("Trying to dequeue with an empty queue!");
		}

		private void SetNextInQueueActive()
		{
			characterPickingMoveQueue.Dequeue();
			
			if (characterPickingMoveQueue.Count == 0)
			{
				SetNoActivePlayer();
				
				// Queue is empty
				EventManager.RaiseEvent(new AllPlayersChoseMoveEvent());
			}
			else
			{
				// Next in line gets to choose a move now
				SetNewActivePlayer(characterPickingMoveQueue.Peek());
			}
		}

		private void SetNewActivePlayer(GameObject player)
		{
			currentlyActivePlayer = player;

			CharacterHealth characterHealth = currentlyActivePlayer.GetComponent<CharacterHealth>();
			
			characterHealth.OnDied += SetNextInQueueActive;

			EventManager.RaiseEvent(new NewPlayerChoosingMoveEvent(player));
		}

		private void SetNoActivePlayer()
		{
			if (currentlyActivePlayer)
			{
				CharacterHealth character = currentlyActivePlayer.GetComponent<CharacterHealth>();
				character.OnDied -= SetNextInQueueActive;
			}

			currentlyActivePlayer = null;

			EventManager.RaiseEvent(new AllPlayersChoseMoveEvent());
		}
	}
}