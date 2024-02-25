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
        private readonly List<CastingState> combatMoveReadyQueue = new();

        private bool isSomeoneCasting;

        private void OnEnable()
        {
            NewCharacterReadyToCastEvent.Listeners += OnNewCharacterReadyToCast;
            NextCombatMoveCanStartEvent.ParameterlessListeners += StartNextInQueue;

            CastingPreventedEvent.Listeners += OnCastingPrevented;
            CombatStartedEvent.ParameterlessListeners += ResetState;
            CombatEndedEvent.ParameterlessListeners += ResetState;
        }

        private void OnDisable()
        {
            NewCharacterReadyToCastEvent.Listeners -= OnNewCharacterReadyToCast;
            NextCombatMoveCanStartEvent.ParameterlessListeners -= StartNextInQueue;
            CastingPreventedEvent.Listeners -= OnCastingPrevented;

            CombatStartedEvent.ParameterlessListeners -= ResetState;
            CombatEndedEvent.ParameterlessListeners -= ResetState;
        }

        private void ResetState()
        {
            combatMoveReadyQueue.Clear();
            isSomeoneCasting = false;
        }

        private void OnNewCharacterReadyToCast(NewCharacterReadyToCastEvent newCharacterReadyToCastEvent)
        {
            HandleNewCharacterReadyToCast(newCharacterReadyToCastEvent.CastingState);
        }

        private void OnCastingPrevented(CastingPreventedEvent castingPreventedEvent)
        {
            RemoveFromQueue(castingPreventedEvent.CastingState);
        }

        private void HandleNewCharacterReadyToCast(CastingState castingState)
        {
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

            combatMoveReadyQueue.Add(castingState);
        }

        private void RemoveFromQueue(CastingState castingState)
        {
            if (!combatMoveReadyQueue.Remove(castingState))
                Debug.LogError("The queue does not contain this character!\n" + castingState.gameObject.name);
        }

        private bool TryGetNextInLine(out CastingState castingState)
        {
            if (combatMoveReadyQueue.Count > 0)
            {
                castingState = combatMoveReadyQueue[0];

                combatMoveReadyQueue.RemoveAt(0);
                return true;
            }

            castingState = null;
            return false;
        }

        private void StartNextInQueue() // Called when a character enters CastingState
        {
            if (TryGetNextInLine(out var castingState)) // Will succeed so long as at least 1 object is in the queue
                castingState.StartCasting();
            else
                isSomeoneCasting = false;
        }
    }
}