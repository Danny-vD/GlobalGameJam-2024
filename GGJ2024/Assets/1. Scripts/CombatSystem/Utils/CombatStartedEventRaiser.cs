using System;
using Cinemachine;
using CombatSystem.Events;
using CombatSystem.Events.EnterCombatArenaEvent;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Utils
{
    public class CombatStartedEventRaiser : BetterMonoBehaviour
    {

        private Animator animator;
        private CinemachineVirtualCamera virtualCamera;
        private int FadeInId = Animator.StringToHash("FadeIn");
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnEnable()
        {
            EventManager.AddListener<EnterCombatArenaEvent>(OnArenaEnter);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<EnterCombatArenaEvent>(OnArenaEnter);
        }

        private void OnArenaEnter(EnterCombatArenaEvent @event)
        {
            virtualCamera.Priority = 1000;  // This is the biggest it should ever be
            animator.Play(FadeInId);
        }

        public void StartCombat()
        {
            EventManager.RaiseEvent<CombatStartedEvent>(new CombatStartedEvent());
        }
    }
}