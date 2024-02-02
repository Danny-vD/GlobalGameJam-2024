using System.Collections.Generic;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Events;
using CombatSystem.Events.Queues;
using UnityEngine;
using VDFramework;

namespace CombatSystem.Managers
{
	public class CombatMoveManager : BetterMonoBehaviour
	{
		// TODO static event<GameObject/CastingState> for when a new move is cast (or do it in castingState)
		private readonly Queue<CastingState> combatMoveReadyQueue = new Queue<CastingState>();

		private bool isSomeoneCasting = false;

		private void OnEnable()
		{
			NewCharacterReadyToCastEvent.Listeners             += OnNewCharacterReadyToCast;
			NextCombatMoveCanStartEvent.ParameterlessListeners += StartNextInQueue;
			CombatStartedEvent.ParameterlessListeners          += ResetState;
			CombatEndedEvent.ParameterlessListeners            += ResetState;
		}

		private void OnDisable()
		{
			NewCharacterReadyToCastEvent.Listeners             -= OnNewCharacterReadyToCast;
			NextCombatMoveCanStartEvent.ParameterlessListeners -= StartNextInQueue;
			CombatStartedEvent.ParameterlessListeners          -= ResetState;
			CombatEndedEvent.ParameterlessListeners            -= ResetState;
		}

		private void ResetState()
		{
			combatMoveReadyQueue.Clear();
			isSomeoneCasting = false;
		}

		private void OnNewCharacterReadyToCast(NewCharacterReadyToCastEvent newCharacterReadyToCastEvent)
		{
			CastingState castingState = newCharacterReadyToCastEvent.ReadyCastingState;

			if (!isSomeoneCasting)
			{
				isSomeoneCasting = true;
				castingState.StartCasting();
			}
			else
			{
				AddToQueue(castingState);
			}
		}

		private void AddToQueue(CastingState castingState)
		{
			if (combatMoveReadyQueue.Contains(castingState))
			{
				Debug.LogError("The queue already contains this character!\n" + castingState.gameObject.name);
				return;
			}

			combatMoveReadyQueue.Enqueue(castingState);
		}

		private void StartNextInQueue() // Called when a character starts casting
		{
			if (combatMoveReadyQueue.TryDequeue(out CastingState castingState)) // Dequeue will succeed so long as at least 1 object is in the queue
			{
				castingState.StartCasting();
			}
			else
			{
				isSomeoneCasting = false;
			}
		}
	}
}