using System;
using System.Collections.Generic;
using CombatSystem.CharacterScripts.CharacterStates;
using CombatSystem.Enums;
using SerializableDictionaryPackage.SerializableDictionary;
using UnityEngine;
using VDFramework;

namespace CombatSystem.CharacterScripts
{
	public class CharacterStateManager : BetterMonoBehaviour
	{
		public event Action OnStateChanged = delegate { };

		[SerializeField]
		private SerializableEnumDictionary<CharacterStateType, AbstractCharacterState> statesPerType;

		public CharacterStateType CurrentStateType { get; private set; }
		
		private List<AbstractCharacterState> states;

		private AbstractCharacterState currentState;

		private void Awake()
		{
			if (!statesPerType.ContainsKey(CharacterStateType.Idle))
			{
				Debug.LogError("No idle state present!");
			}

			SetState(CharacterStateType.Idle);
		}

		private void Update()
		{
			currentState.Step();
		}

		private void SetState(CharacterStateType stateType)
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
	}
}