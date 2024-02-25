using CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
    public class DeadState : AbstractCharacterState
    {
        private CharacterHealth characterHealth;

        private Sprite originalSprite;
        public override CharacterCombatStateType NextState => CharacterCombatStateType.Idle;

        private void Awake()
        {
            characterHealth = GetComponent<CharacterHealth>();

            characterHealth.OnResurrected += Exit;
        }

        private void OnDestroy()
        {
            characterHealth.OnResurrected -= Exit;
        }

        public override void Enter()
        {
        }
    }
}