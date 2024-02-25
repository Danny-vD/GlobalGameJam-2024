using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events.CharacterStateEvents
{
    public class PlayerExitedChoosingStateEvent : VDEvent<PlayerExitedChoosingStateEvent>
    {
        public readonly GameObject Player;

        public PlayerExitedChoosingStateEvent(GameObject player)
        {
            Player = player;
        }
    }
}