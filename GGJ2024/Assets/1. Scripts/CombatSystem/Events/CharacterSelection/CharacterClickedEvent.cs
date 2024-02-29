using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events.CharacterSelection
{
    public class CharacterClickedEvent : VDEvent<CharacterClickedEvent>
    {
        public readonly GameObject Character;

        public CharacterClickedEvent(GameObject character)
        {
            Character = character;
        }
    }
}