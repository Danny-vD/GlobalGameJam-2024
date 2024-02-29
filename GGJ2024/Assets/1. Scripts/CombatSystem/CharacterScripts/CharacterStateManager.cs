using System;
using System.Collections.Generic;
using CharacterScripts;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Enums;
using CombatSystem.Events;
using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework;
using VDFramework.UnityExtensions;

namespace CombatSystem.CharacterScripts
{
    public class CharacterStateManager : BetterMonoBehaviour
    {
        [SerializeField]
        private SerializableEnumDictionary<CharacterCombatStateType, AbstractCharacterState> statesPerType;

        private CharacterHealth characterHealth;

        private AbstractCharacterState currentState;

        private List<AbstractCharacterState> states;

        public CharacterCombatStateType CurrentStateType { get; private set; }

        private void Awake()
        {
            if (statesPerType[CharacterCombatStateType.Idle] == null)
            {
                Debug.LogError("No idle state present!");
            }

            CombatStartedEvent.ParameterlessListeners += this.Enable;
            CombatEndedEvent.ParameterlessListeners += this.Disable;

            characterHealth = GetComponent<CharacterHealth>();

            this.Disable();
        }

        private void Update()
        {
            currentState.Step();
        }

        private void OnEnable()
        {
            SetState(CharacterCombatStateType.Idle);

            characterHealth.OnDied += ForceDeadState;
        }

        private void OnDisable()
        {
            characterHealth.OnDied -= ForceDeadState;
        }

        private void OnDestroy()
        {
            if (currentState != null) currentState.OnStateEnded -= SetState;

            CombatStartedEvent.ParameterlessListeners -= this.Enable;
            CombatEndedEvent.ParameterlessListeners -= this.Disable;
        }

        public event Action<CharacterCombatStateType>
            OnStateChanged = delegate
            {
            }; // TODO animation components that listen to this event and set animation triggers accordingly

        public void RestartCurrentState()
        {
            ForceState(CurrentStateType);
        }

        /// <summary>
        ///     Forcibly exits the current state and starts a new one
        /// </summary>
        public void ForceState(CharacterCombatStateType stateType)
        {
            currentState.OnStateEnded -=
                SetState; // Stop reacting to the current state ending so that we do not set up the state that would've come after
            currentState.Exit();

            SetState(stateType);
        }

        private void SetState(CharacterCombatStateType stateType)
        {
            if (currentState != null) currentState.OnStateEnded -= SetState;

            CurrentStateType = stateType;

            currentState = statesPerType[CurrentStateType];
            OnStateChanged.Invoke(stateType);

            currentState.OnStateEnded += SetState;

            currentState.Enter();
        }

        private void ForceDeadState()
        {
            ForceState(CharacterCombatStateType.Dead);
        }

        private void ForceStunnedState()
        {
            ForceState(CharacterCombatStateType.Stunned);
        }
    }
}