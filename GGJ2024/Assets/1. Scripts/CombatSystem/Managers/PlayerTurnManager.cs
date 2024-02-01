using System;
using System.Collections.Generic;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Events;
using CombatSystem.Events.CharacterStateEvents;
using UnityEditor;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Managers
{
	public class PlayerTurnManager : BetterMonoBehaviour
	{
		public static event Action<GameObject> NewCharacterChoosingMove = delegate { };
		public static event Action OnChoosingQueueEmpty = delegate { };

		private readonly Queue<GameObject> characterPickingMoveQueue = new Queue<GameObject>();

		private void OnEnable()
		{
			PlayerStartedChoosingEvent.Listeners += AddToQueue;
			PlayerStoppedChoosingEvent.Listeners += RemoveFromQueue;
			
			CombatStartedEvent.ParameterlessListeners += ResetState;
			CombatEndedEvent.ParameterlessListeners   += ResetState;
		}

		private void OnDisable()
		{
			PlayerStartedChoosingEvent.Listeners -= AddToQueue;
			PlayerStoppedChoosingEvent.Listeners -= RemoveFromQueue;
			
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

		private void AddToQueue(PlayerStartedChoosingEvent startedChoosingEvent)
		{
			GameObject player = startedChoosingEvent.Player;
			
			if (characterPickingMoveQueue.Contains(player))
			{
				Debug.LogError("The queue already contains this character!\n" + player.name);
				return;
			}

			characterPickingMoveQueue.Enqueue(player);

			if (characterPickingMoveQueue.Count == 1)
			{
				// First character added so it is always going to be the one that is choosing a move
				NewCharacterChoosingMove.Invoke(player);
			}
		}

		private void RemoveFromQueue(PlayerStoppedChoosingEvent stoppedChoosingEvent)
		{
			if (characterPickingMoveQueue.TryPeek(out GameObject next))
			{
				GameObject player = stoppedChoosingEvent.Player;

				if (ReferenceEquals(player, next))
				{
					characterPickingMoveQueue.Dequeue();

					if (characterPickingMoveQueue.Count == 0)
					{
						// Queue is empty
						OnChoosingQueueEmpty.Invoke();
					}
					else
					{
						// Next in line gets to choose a move now
						NewCharacterChoosingMove.Invoke(characterPickingMoveQueue.Peek());
					}

					return;
				}

				Debug.LogError($"Attempting to dequeue the gameobject that is not next in line!\ndequeuing: {player.name}\tNext: {next.name}");
				return;
			}

			Debug.LogError("Trying to dequeue with an empty queue!");
		}
	}
}