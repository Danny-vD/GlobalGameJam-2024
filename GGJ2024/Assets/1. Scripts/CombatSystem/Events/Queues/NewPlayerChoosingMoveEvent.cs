using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events.Queues
{
    public class NewPlayerChoosingMoveEvent : VDEvent<NewPlayerChoosingMoveEvent>
    {
        public readonly GameObject Player;

        public NewPlayerChoosingMoveEvent(GameObject player)
        {
            Player = player;
        }
    }
}