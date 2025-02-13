﻿using CombatSystem.Enums;
using CombatSystem.Events.CharacterStateEvents;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.CharacterScripts.CharacterStates
{
    [RequireComponent(typeof(ConfirmedMoveHolder))]
    public class PlayerChoosingState : AbstractCharacterState
    {
        private ConfirmedMoveHolder confirmedMoveHolder;
        public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

        private void Awake()
        {
            confirmedMoveHolder = GetComponent<ConfirmedMoveHolder>();

            confirmedMoveHolder.OnMoveSelected += Exit;
        }

        private void OnDestroy()
        {
            confirmedMoveHolder.OnMoveSelected -= Exit;
        }

        public override void Enter()
        {
            EventManager.RaiseEvent(new PlayerEnteredChoosingStateEvent(gameObject));
        }

        public override void Exit()
        {
            EventManager.RaiseEvent(new PlayerExitedChoosingStateEvent(gameObject));
            base.Exit();
        }
    }
}