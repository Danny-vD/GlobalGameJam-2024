using System.Collections.Generic;
using CharacterScripts;
using CombatSystem.CharacterScripts;
using CombatSystem.Enums;
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

			AddToQueue(player);
		}

		private void AddToQueue(GameObject player)
		{
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
			RemoveFromQueue(exitedChoosingStateEvent.Player);
		}

		private void RemoveFromQueue(GameObject player)
		{
			if (characterPickingMoveQueue.TryPeek(out GameObject next))
			{
				if (ReferenceEquals(player, next))
				{
					SetNextInQueueActive();

					return;
				}

				Debug.LogError($"Attempting to dequeue the gameobject that is not next in line!\ndequeuing: {player.name}\tNext: {next.name}");
				return;
			}

			Debug.LogError("Trying to dequeue with an empty queue! " + player.name);
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
				GameObject player = characterPickingMoveQueue.Peek();
				CharacterStateManager characterStateManager = player.GetComponent<CharacterStateManager>();

				bool isValidPlayer = characterStateManager.CurrentStateType is CharacterCombatStateType.Dead or CharacterCombatStateType.Stunned;

				if (isValidPlayer)
				{
					SetNewActivePlayer(player, characterStateManager);
				}
				else
				{
					SetNextInQueueActive();
				}
			}
		}

		private void SetNewActivePlayer(GameObject player, CharacterStateManager characterStateManager = null)
		{
			currentlyActivePlayer = player;

			if (!characterStateManager)
			{
				characterStateManager = currentlyActivePlayer.GetComponent<CharacterStateManager>();
			}

			characterStateManager.OnStateChanged += OnActiveCharacterStateChanged;

			EventManager.RaiseEvent(new NewPlayerChoosingMoveEvent(player));
		}

		private void SetNoActivePlayer()
		{
			if (currentlyActivePlayer)
			{
				CharacterStateManager characterStateManager = currentlyActivePlayer.GetComponent<CharacterStateManager>();
				characterStateManager.OnStateChanged -= OnActiveCharacterStateChanged;
			}

			currentlyActivePlayer = null;

			EventManager.RaiseEvent(new AllPlayersChoseMoveEvent());
		}

		private void OnActiveCharacterStateChanged(CharacterCombatStateType combatState)
		{
			if (combatState is CharacterCombatStateType.Dead or CharacterCombatStateType.Stunned)
			{
				SetNextInQueueActive();
			}
		}
	}
}