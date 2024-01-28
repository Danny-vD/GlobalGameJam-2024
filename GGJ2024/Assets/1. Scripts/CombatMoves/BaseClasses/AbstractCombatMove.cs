using System;
using CombatMoves.TargetingLogic.Enums;
using CombatMoves.TargetingLogic.TargetingValidators.Util;
using CombatSystem.Enums;
using UnityEngine;
using VDFramework;

namespace CombatMoves.BaseClasses
{
	public abstract class AbstractCombatMove : BetterMonoBehaviour
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
		public string AnimationTriggerName { get; private set; }

		public bool IsValidTarget(GameObject target)
		{
			return TargetingValidatorUtil.GetValidators(ValidTargets).TrueForAll(validator => validator.IsValidTarget(target, gameObject));
		}

		public abstract void StartCombatMove(GameObject target);
	}
}