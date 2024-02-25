using CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
    [RequireComponent(typeof(Character))]
    public class IdleState : AbstractCharacterState
    {
        private Character character;
        private CharacterStaminaTimer characterStamina;
        public override CharacterCombatStateType NextState => CharacterCombatStateType.Choosing;

        private void Awake()
        {
            character = GetComponent<Character>();
            characterStamina = GetComponent<CharacterStaminaTimer>();
        }

        public override void Enter()
        {
            characterStamina.ResetStamina();
            characterStamina.OnStaminaTimerExpired += Exit;
        }

        public override void Step()
        {
            characterStamina.DecreaseStamina(character.Attributes.Speed * Time.deltaTime);
        }

        public override void Exit()
        {
            characterStamina.OnStaminaTimerExpired -= Exit;
            base.Exit();
        }
    }
}