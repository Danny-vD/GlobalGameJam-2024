using CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(Character))]
	public class IdleState : AbstractCharacterState
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
			characterStamina.DecreaseStamina(character.Statistics.Speed * Time.deltaTime);
		}

		private void OnDestroy()
		{
			characterStamina.OnStaminaTimerExpired -= Exit;
		}
	}
}