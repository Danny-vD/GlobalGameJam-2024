using System;
using System.Collections.Generic;
using CombatSystem.Events;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Triggers
{
    public class CombatTrigger : BetterMonoBehaviour
    {

        [SerializeField] private List<GameObject> Enemies;
        private void OnTriggerEnter(Collider other)
        {
            EventManager.RaiseEvent(new CombatStartedEvent(Enemies));
        }
    }
}