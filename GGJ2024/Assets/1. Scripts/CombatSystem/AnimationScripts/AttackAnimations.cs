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
			animator            = GetComponent<Animator>();
			castingState        = GetComponentInParent<CastingState>();
			confirmedMoveHolder = GetComponentInParent<ConfirmedMoveHolder>();
		}

		private void OnEnable()
		{
			castingState.OnCastingStarted += OnStartedCasting;
		}

		private void OnDisable()
		{
			castingState.OnCastingStarted -= OnStartedCasting;
		}

		private void OnStartedCasting()
		{
			// Getting the animation trigger name from the move allows us to set the correct animation per move in just 1 line
			animator.SetTrigger(GetParameterID(confirmedMoveHolder.SelectedMove.AnimationTriggerName));
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