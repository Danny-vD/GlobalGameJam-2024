using CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
    public class StunnedState : AbstractCharacterState
    {
        private Character character;
        private CharacterStaminaTimer characterStamina;
        public override CharacterCombatStateType NextState => CharacterCombatStateType.Choosing;

        private void Awake()
        {
            character = GetComponent<Character>();
            characterStamina = GetComponent<CharacterStaminaTimer>();

            characterStamina.OnStaminaTimerExpired += Exit;
        }

        private void OnDestroy()
        {
            characterStamina.OnStaminaTimerExpired -= Exit;
        }

        public override void Enter()
        {
            characterStamina.ResetStamina();
        }

        public override void Step()
        {
            // Stun is basically a slower idle
            characterStamina.DecreaseStamina(character.Attributes.Speed / 2 * Time.deltaTime); // TODO: make the time a variable
        }
    }
}