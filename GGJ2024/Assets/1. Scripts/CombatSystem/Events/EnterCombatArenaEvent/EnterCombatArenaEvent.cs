using System.Collections.Generic;
using CombatSystem.Enums;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace CombatSystem.Events.EnterCombatArenaEvent
{
    public class EnterCombatArenaEvent : VDEvent<EnterCombatArenaEvent>
    {
        public readonly CombatTypes CombatType;
        public readonly ArenaTypes ArenaType;
        
        public readonly List<GameObject> Enemies;
        public readonly List<GameObject> PartyMembers;

        public EnterCombatArenaEvent(List<GameObject> partyMembers, List<GameObject> enemies, ArenaTypes arenaType, CombatTypes combatType)
        {
            PartyMembers = partyMembers;
            Enemies = enemies;
            ArenaType = arenaType;
            CombatType = combatType;
        }
    }
}