﻿using System;
using CombatSystem.Enums;
using VDFramework;

namespace CombatSystem.CharacterScripts.CharacterStates
{
    public abstract class AbstractCharacterState : BetterMonoBehaviour
    {
        public abstract CharacterCombatStateType NextState { get; }
        public event Action<CharacterCombatStateType> OnStateEnded = delegate { };

        public abstract void Enter();

        public virtual void Step()
        {
        }

        public virtual void Exit()
        {
            OnStateEnded.Invoke(NextState);
        }
    }
}