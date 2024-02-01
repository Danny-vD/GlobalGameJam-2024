using System;
using CombatMoves.TargetingLogic.Enums;
using CombatMoves.TargetingLogic.TargetingValidators.Util;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatMoves.BaseClasses
{
	public abstract class AbstractCombatMove : ScriptableObject
	{
		public event Action OnCombatMoveEnded = delegate { };

		[Header("General data")]
		[field: SerializeField]
		public string AbilityName { get; protected set; }

		[field: SerializeField]
		public string Description { get; protected set; }

		[field: SerializeField]
		public int Cost { get; protected set; } = 0;

		[Header("Damage")]
		[field: SerializeField]
		public DamageType DamageType { get; protected set; } = DamageType.Normal;

		[field: SerializeField]
		public float Potency { get; protected set; } = 10;

		[field: SerializeField]
		public ValidTargets ValidTargets { get; protected set; } = ValidTargets.OpposingTeam;

		[Header("Animation")]
		[field: SerializeField]
		public string AnimationTriggerName { get; protected set; }

		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			//TODO: Cache the ITargetingValidator
			return TargetingValidatorUtil.GetValidators(ValidTargets).IsValidTarget(target, caster);
		}
		
		public abstract void StartCombatMove(GameObject target, GameObject caster);

		// TODO: Change so that we can say that someone else can start without explicitely ending our own casting yet (e.g. for charging/longer resting)
		protected void InvokeCombatMoveEnded()
		{
			OnCombatMoveEnded.Invoke();
		}
	}
}