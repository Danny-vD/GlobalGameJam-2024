using System.Collections.Generic;
using CombatSystem.Events;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;
using VDFramework.UnityExtensions;

namespace CombatSystem.Triggers
{
    public class CombatTrigger : BetterMonoBehaviour
    {
        [SerializeField] private Collider combatCollider;

        [SerializeField] private List<GameObject> Enemies;

        private void OnTriggerEnter(Collider other)
        {
            EventManager.RaiseEvent(new CombatStartedEvent(Enemies));
            combatCollider.Disable();
            this.Disable();
        }
    }
}