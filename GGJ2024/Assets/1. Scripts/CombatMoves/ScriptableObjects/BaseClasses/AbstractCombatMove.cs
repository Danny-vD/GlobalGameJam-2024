using System;
using System.Collections.Generic;
using CombatMoves.TargetingLogic.Enums;
using CombatMoves.TargetingLogic.Interfaces;
using CombatMoves.TargetingLogic.TargetingValidators.Util;
using CombatSystem.Enums;
using CombatSystem.Events.Queues;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatMoves.ScriptableObjects.BaseClasses
{
	public abstract class AbstractCombatMove : ScriptableObject
	{
		[Header("General data")]
		[field: SerializeField]
		public string AbilityName { get; protected set; }

		[field: SerializeField]
		public string Description { get; protected set; }

		[field: SerializeField]
		public int Cost { get; protected set; }

		[Header("Damage")]
		[field: SerializeField]
		public DamageType DamageType { get; protected set; } = DamageType.Normal;

		[field: SerializeField]
		public float Potency { get; protected set; } = 10;

		[field: SerializeField]
		public ValidTargets ValidTargets { get; protected set; } = ValidTargets.OpposingTeam;

		[field: SerializeField]
		public TargetingMode TargetingMode { get; protected set; } = TargetingMode.MultipleTargets;

		[Header("Animation")]
		[field: SerializeField]
		public string AnimationTriggerName { get; protected set; }

		private ITargetingValidator cachedValidator;

		private ValidTargets oldValidTargets;
		public event Action<GameObject> OnCombatMoveEnded = delegate { };

		/// <summary>
		///     Allows another combat move to start
		/// </summary>
		protected static void AllowNextMoveToStart()
		{
			EventManager.RaiseEvent(new NextCombatMoveCanStartEvent());
		}

		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			return GetTargetingValidator().IsValidTarget(target, caster);
		}

		/// <summary>
		///     Start performing this combat move
		/// </summary>
		/// <param name="targets">The targets of the move</param>
		/// <param name="caster">The gameobject that casts the move</param>
		public abstract void StartCombatMove(List<GameObject> targets, GameObject caster);

		/// <summary>
		///     Immediately interrupts and stops the combat move
		/// </summary>
		public virtual void ForceStopCombatMove(GameObject caster)
		{
			// If we died/got stunned while performing a move, we already allowed the next one to start, hence no AllowNextMoveToStart
			InvokeOnCombatMoveEnded(caster);
		}

		/// <summary>
		///     Tells the casting state that the move ended and allows another move to start
		/// </summary>
		/// <seealso cref="AllowNextMoveToStart" />
		/// <seealso cref="InvokeOnCombatMoveEnded" />
		protected void EndCombatMove(GameObject caster)
		{
			AllowNextMoveToStart();
			InvokeOnCombatMoveEnded(caster);
		}

		/// <summary>
		///     Invokes <see cref="OnCombatMoveEnded" />
		/// </summary>
		protected void InvokeOnCombatMoveEnded(GameObject caster)
		{
			OnCombatMoveEnded.Invoke(caster);
		}

		private ITargetingValidator GetTargetingValidator()
		{
			if (cachedValidator == null || oldValidTargets != ValidTargets) // Check if valid targets changed
			{
				oldValidTargets = ValidTargets;
				cachedValidator = TargetingValidatorUtil.GetValidators(ValidTargets);
			}

			return cachedValidator;
		}
	}
}