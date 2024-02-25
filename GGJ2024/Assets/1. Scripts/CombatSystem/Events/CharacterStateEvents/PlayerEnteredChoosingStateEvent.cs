using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events.CharacterStateEvents
{
    public class PlayerEnteredChoosingStateEvent : VDEvent<PlayerEnteredChoosingStateEvent>
    {
        public readonly GameObject Player;

        public PlayerEnteredChoosingStateEvent(GameObject player)
        {
            Player = player;
        }
    }
}