using System;
using System.Collections.Generic;
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

		private void Awake()
		{
			if (!statesPerType.ContainsKey(CharacterCombatStateType.Idle))
			{
				Debug.LogError("No idle state present!");
			}

			CombatStartedEvent.ParameterlessListeners += this.Enable;
			CombatEndedEvent.ParameterlessListeners   += this.Disable;

			this.Disable();
		}

		private void Start()
		{
			SetState(CharacterCombatStateType.Idle);
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