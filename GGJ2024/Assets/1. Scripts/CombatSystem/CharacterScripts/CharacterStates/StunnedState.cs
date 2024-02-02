using CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class StunnedState : AbstractCharacterState
	{
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Choosing;

		private Character character;
		private CharacterStaminaTimer characterStamina;

		private void Awake()
		{
			character        = GetComponent<Character>();
			characterStamina = GetComponent<CharacterStaminaTimer>();

			characterStamina.OnStaminaTimerExpired += Exit;
		}

		public override void Enter()
		{
			characterStamina.ResetStamina();
		}

		public override void Step()
		{
			// Stun is basically a slower idle
			characterStamina.DecreaseStamina(character.Statistics.Speed / 2 * Time.deltaTime);
		}

		private void OnDestroy()
		{
			characterStamina.OnStaminaTimerExpired -= Exit;
		}
	}
}