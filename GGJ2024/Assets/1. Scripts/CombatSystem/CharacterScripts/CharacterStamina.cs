using System;
using UnityEngine;
using VDFramework;

namespace CombatSystem.CharacterScripts
{
	public class CharacterStaminaTimer : BetterMonoBehaviour
	{
		[field: SerializeField]
		public float MaximumStaminaTimer { get; private set; } = 10;

		public float NormalizedStaminaTimer => StaminaTmer / MaximumStaminaTimer;

		public float StaminaTmer { get; private set; }
		public event Action OnStaminaTimerExpired = delegate { };
		public event Action OnStaminaTimerReset = delegate { };

		public void ResetStamina()
		{
			StaminaTmer = MaximumStaminaTimer;

			OnStaminaTimerReset.Invoke();
		}

		public void DecreaseStamina(float value)
		{
			StaminaTmer -= value;

			if (StaminaTmer <= 0)
			{
				StaminaTmer = 0;
				OnStaminaTimerExpired.Invoke();
			}
		}
	}
}