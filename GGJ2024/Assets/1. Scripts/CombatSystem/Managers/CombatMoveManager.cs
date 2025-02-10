using System.Collections.Generic;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Events;
using CombatSystem.Events.Queues;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Managers
{
    public class CombatMoveManager : BetterMonoBehaviour
    {
        private readonly List<CastingState> combatMoveReadyQueue = new List<CastingState>();

        private CastingState lastCaster = null; // Will be null if no one is currently casting
        private bool isSomeoneCasting;

        private void OnEnable()
        {
            NewCharacterReadyToCastEvent.Listeners += OnNewCharacterReadyToCast;
            NextCombatMoveCanStartEvent.ParameterlessListeners += StartNextInQueue;
            
            CastingPreventedEvent.Listeners += OnCastingPrevented;
            CastingInterupptedEvent.Listeners += OnCastingInteruppted;

            CombatStartedEvent.ParameterlessListeners += ResetState;
            CombatEndedEvent.ParameterlessListeners += ResetState;
        }

        private void OnDisable()
        {
            NewCharacterReadyToCastEvent.Listeners -= OnNewCharacterReadyToCast;
            NextCombatMoveCanStartEvent.ParameterlessListeners -= StartNextInQueue;
            
            CastingPreventedEvent.Listeners   -= OnCastingPrevented;
            CastingInterupptedEvent.Listeners -= OnCastingInteruppted;

            CombatStartedEvent.ParameterlessListeners -= ResetState;
            CombatEndedEvent.ParameterlessListeners -= ResetState;
        }

        private void ResetState()
        {
            combatMoveReadyQueue.Clear();
            
            lastCaster       = null;
            isSomeoneCasting = false;
        }

        private void OnNewCharacterReadyToCast(NewCharacterReadyToCastEvent newCharacterReadyToCastEvent)
        {
            HandleNewCharacterReadyToCast(newCharacterReadyToCastEvent.CastingState);
        }

        private void OnCastingInteruppted(CastingInterupptedEvent castingInterupptedEvent)
        {
            RemoveFromQueue(castingInterupptedEvent.CastingState);
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
                lastCaster       = castingState;
                
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
            {
                Debug.LogError("The queue does not contain this character!\n" + castingState.gameObject.name);
            }

            if (ReferenceEquals(lastCaster, castingState))
            {
                lastCaster = null; // Technically unneccessary to set this here (it's immediately overwritten)
                EventManager.RaiseEvent(new NextCombatMoveCanStartEvent());
            }
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
            if (TryGetNextInLine(out CastingState castingState)) // Will succeed if at least 1 object is in the queue
            {
                lastCaster = castingState;

                // isSomeoneCasting = true; is unnecessary as it is already true
                castingState.StartCasting();
            }
            else
            {
                lastCaster       = null;
                isSomeoneCasting = false;
            }
        }
    }
}