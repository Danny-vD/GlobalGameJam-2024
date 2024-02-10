using System;
using System.Collections.Generic;
using CombatMoves.TargetingLogic.Enums;
using CombatMoves.TargetingLogic.TargetingValidators.Util;
using CombatSystem.Enums;
using CombatSystem.Events.Queues;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatMoves.ScriptableObjects.BaseClasses
{
	public abstract class AbstractCombatMove : ScriptableObject
	{
		public event Action OnCombatMoveEnded = delegate { }; // TODO: Test whether this delegate is shared between characters

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

		/// <summary>
		/// Allows another combat move to start
		/// </summary>
		protected static void AllowNextMoveToStart()
		{
			EventManager.RaiseEvent(new NextCombatMoveCanStartEvent());
		}

		public bool IsValidTarget(GameObject target, GameObject caster)
		{
			//TODO: Cache the ITargetingValidator
			return TargetingValidatorUtil.GetValidators(ValidTargets).IsValidTarget(target, caster);
		}

		/// <summary>
		/// Start performing this combat move
		/// </summary>
		/// <param name="targets">The targets of the move</param>
		/// <param name="caster">The gameobject that casts the move</param>
		public abstract void StartCombatMove(List<GameObject> targets, GameObject caster);

		/// <summary>
		/// Immediately interrupts and stops the combat move
		/// </summary>
		public virtual void ForceStopCombatMove()
		{
			// If we died/got stunned while performing a move, we already allowed the next one to start
			InvokeOnCombatMoveEnded();
		}

		/// <summary>
		/// Tells the casting state that the move ended and allows another move to start
		/// </summary>
		/// <seealso cref="AllowNextMoveToStart"/>
		/// <seealso cref="InvokeOnCombatMoveEnded"/>
		protected void EndCombatMove()
		{
			AllowNextMoveToStart();
			InvokeOnCombatMoveEnded();
		}

		/// <summary>
		/// Invokes <see cref="OnCombatMoveEnded"/>
		/// </summary>
		protected void InvokeOnCombatMoveEnded()
		{
			OnCombatMoveEnded.Invoke();
		}
	}
}