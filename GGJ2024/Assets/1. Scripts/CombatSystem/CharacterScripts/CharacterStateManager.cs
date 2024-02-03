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
		public event Action OnStateChanged = delegate { };

		[SerializeField]
		private SerializableEnumDictionary<CharacterCombatStateType, AbstractCharacterState> statesPerType;

		public CharacterCombatStateType CurrentStateType { get; private set; }

		private List<AbstractCharacterState> states;

		private AbstractCharacterState currentState;
		private CharacterHealth characterHealth;

		private void Awake()
		{
			if (!statesPerType.ContainsKey(CharacterCombatStateType.Idle))
			{
				Debug.LogError("No idle state present!");
			}

			CombatStartedEvent.ParameterlessListeners += this.Enable;
			CombatEndedEvent.ParameterlessListeners   += this.Disable;

			characterHealth = GetComponent<CharacterHealth>();

			this.Disable();
		}

		private void OnEnable()
		{
			SetState(CharacterCombatStateType.Idle);
			
			characterHealth.OnDied        += SetToDeadState;
			characterHealth.OnResurrected += SetToIdleState;
		}

		private void OnDisable()
		{
			characterHealth.OnDied        -= SetToDeadState;
			characterHealth.OnResurrected -= SetToIdleState;
		}

		private void Update()
		{
			currentState.Step();
		}
		
		private void SetState(CharacterCombatStateType stateType)
		{
			if (currentState != null)
			{
				currentState.OnStateEnded -= SetState;
			}

			CurrentStateType = stateType;

			currentState = statesPerType[CurrentStateType];
			OnStateChanged.Invoke();

			currentState.OnStateEnded += SetState;

			currentState.Enter();
		}

		private void SetToDeadState()
		{
			SetState(CharacterCombatStateType.Dead);
		}

		private void SetToStunState()
		{
			SetState(CharacterCombatStateType.Stunned);
		}

		private void SetToIdleState()
		{
			SetState(CharacterCombatStateType.Idle);
		}

		private void OnDestroy()
		{
			if (currentState != null)
			{
				currentState.OnStateEnded -= SetState;
			}

			CombatStartedEvent.ParameterlessListeners -= this.Enable;
			CombatEndedEvent.ParameterlessListeners   -= this.Disable;
		}
	}
}