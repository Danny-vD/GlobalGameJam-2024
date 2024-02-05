using System.Collections.Generic;
using CombatSystem.CharacterScripts;
using CombatSystem.CharacterScripts.CharacterStates;
using UnityEngine;
using VDFramework;

namespace CombatSystem.AnimationScripts
{
	public class AttackAnimations : BetterMonoBehaviour
	{
		private static readonly Dictionary<string, int> parameterIDs = new Dictionary<string, int>();
		
		private Animator animator;
		private CastingState castingState;
		private ConfirmedMoveHolder confirmedMoveHolder;

		private void Awake()
		{
			animator           = GetComponent<Animator>();
			castingState       = GetComponent<CastingState>();
			confirmedMoveHolder = GetComponent<ConfirmedMoveHolder>();
			
			castingState.OnCastingStarted += OnStartedCasting;
		}

		private void OnStartedCasting()
		{
			// Using the move name as the trigger allows us to dynamically set the respective animation
			//animator.SetTrigger(GetParameterID(confirmedMoveHolder.SelectedMove.AnimationTriggerName));
		}

		private void OnDestroy()
		{
			castingState.OnCastingStarted += OnStartedCasting;
		}

		private static int GetParameterID(string combatMoveName)
		{
			if (parameterIDs.TryGetValue(combatMoveName, out int id))
			{
				return id;
			}

			id = Animator.StringToHash(combatMoveName);
			
			parameterIDs.Add(combatMoveName, id);

			return id;
		}
	}
}