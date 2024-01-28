using System;
using System.Collections.Generic;
using CombatSystem.CharacterScripts.CharacterStates;
using UnityEngine;
using VDFramework;

namespace CombatSystem.Managers
{
	public class CombatMoveManager : BetterMonoBehaviour //TODO: Keep track of all the moves and apply them one by one
	{
		private readonly Queue<Action> combatMoveReadyQueue = new Queue<Action>();

		private void OnEnable()
		{
			CastingState.OnNewCharacterReadyToCast  += OnNewCharacterReadyToCast;
			CastingState.OnCharacterFinishedCasting += RemoveFromQueue;
		}

		private void OnDisable()
		{
			CastingState.OnNewCharacterReadyToCast  -= OnNewCharacterReadyToCast;
			CastingState.OnCharacterFinishedCasting -= RemoveFromQueue;
		}

		private void OnNewCharacterReadyToCast(Action functionToCall)
		{
			if (combatMoveReadyQueue.Count == 0)
			{
				functionToCall.Invoke();
			}
			
			AddToQueue(functionToCall);
		}

		private void AddToQueue(Action functionToCall)
		{
			if (combatMoveReadyQueue.Contains(functionToCall))
			{
				Debug.LogError("The queue already contains this character!");
				return;
			}

			combatMoveReadyQueue.Enqueue(functionToCall);
		}

		private void RemoveFromQueue()
		{
			combatMoveReadyQueue.Dequeue();
			
			if (combatMoveReadyQueue.TryPeek(out Action functionToCall))
			{
				functionToCall.Invoke();
			}
		}
	}
}