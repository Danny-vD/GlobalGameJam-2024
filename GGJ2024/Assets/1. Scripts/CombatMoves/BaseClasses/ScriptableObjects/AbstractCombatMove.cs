using System;
using CombatMoves.TargetingLogic.Enums;
using CombatMoves.TargetingLogic.TargetingValidators.Util;
using CombatSystem.Enums;
using CombatSystem.Events.Queues;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatMoves.BaseClasses.ScriptableObjects
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
		/// <param name="target">The target of the move</param>
		/// <param name="caster">The gameobject that casts the move</param>
		public abstract void StartCombatMove(GameObject target, GameObject caster);

		/// <summary>
		/// Immediately interrupts and stops the combat move
		/// </summary>
		public virtual void ForceStopCombatMove() // NOTE Public so that it can be directly called from Dead/Stun state
		{
			EndCombatMove();
		}
		
		/// <summary>
		/// Invokes all events at once
		/// </summary>
		/// <seealso cref="AllowNextMoveToStart"/>
		/// <seealso cref="InvokeOnCombatMoveEnded"/>
		protected void EndCombatMove()
		{
			InvokeOnCombatMoveEnded();
			AllowNextMoveToStart();
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