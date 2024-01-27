using CombatSystem.CharacterScripts.CharacterStates;
using UnityEngine;
using VDFramework;

namespace CombatSystem.AnimationScripts
{
	public class AttackAnimations : BetterMonoBehaviour
	{
		private static readonly int castingStarted = Animator.StringToHash("CastingStarted");
		
		private Animator animator;
		private CastingState castingState;

		private void Awake()
		{
			animator     = GetComponent<Animator>();
			castingState = GetComponent<CastingState>();
			
			castingState.OnCastingStarted += CastingStateOnOnCastingStarted;
		}

		private void CastingStateOnOnCastingStarted()
		{
			animator.SetTrigger(castingStarted);
		}

		private void OnDestroy()
		{
			castingState.OnCastingStarted += CastingStateOnOnCastingStarted;
		}
	}
}