using System;
using System.Collections.Generic;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Events;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Managers
{
	public class PlayerTurnManager : BetterMonoBehaviour //TODO: Keep track of all the moves and apply them one by one
	{
		public static event Action<GameObject> NewCharacterChoosingMove = delegate { };
		public static event Action OnChoosingQueueEmpty = delegate { };

		private readonly Queue<GameObject> characterPickingMoveQueue = new Queue<GameObject>();

		private void Awake()
		{
			PlayerChoosingState.StartedChoosingState += AddToQueue;
			PlayerChoosingState.EndedChoosingState   += RemoveFromQueue;
		}

		[ContextMenu("Start combat")] //TODO: remove
		private void DebugStartCombat()
		{
			EventManager.RaiseEvent(new CombatStartedEvent(null));
		}

		private void AddToQueue(GameObject obj)
		{
			Debug.Log($"Count: {characterPickingMoveQueue.Count}\n{obj.name}");
			
			if (characterPickingMoveQueue.Contains(obj))
			{
				Debug.LogError("The queue already contains this character!\n" + obj.name);
				return;
			}

			characterPickingMoveQueue.Enqueue(obj);

			if (characterPickingMoveQueue.Count == 1)
			{
				// First character added so it is always going to be the one that is choosing a move
				NewCharacterChoosingMove.Invoke(obj);
			}
		}

		private void RemoveFromQueue(GameObject obj)
		{
			if (characterPickingMoveQueue.TryPeek(out GameObject next))
			{
				if (ReferenceEquals(obj, next))
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

				Debug.LogError($"Attempting to dequeue the gameobject that is not next in line!\ndequeuing: {obj.name}\tNext: {next.name}");
				return;
			}

			Debug.LogError("Trying to dequeue with an empty queue!");
		}

		private void OnDestroy()
		{
			PlayerChoosingState.StartedChoosingState -= AddToQueue;
			PlayerChoosingState.EndedChoosingState   -= RemoveFromQueue;
		}
	}
}