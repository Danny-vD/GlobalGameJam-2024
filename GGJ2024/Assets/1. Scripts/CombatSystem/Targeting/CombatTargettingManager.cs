﻿using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Events;
using CombatSystem.Events.CharacterSelection;
using InputScripts;
using PlayerPartyScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace CombatSystem.Managers.TargettingSystem
{
    public class
        CombatTargettingManager : Singleton<CombatTargettingManager> // TODO: override target selection when taunted?? (Can a player be taunted?)
    {
        [SerializeField] private GameObject currentSelectedCharacterIndicator;

        private GameObject casterToBe;

        private readonly List<GameObject> characterList = new();

        private CombatManager combatManager;

        private bool targetsChoosen;

        private AbstractCombatMove toBeConfirmedMove;
        private List<GameObject> selectedTargets;

        protected override void Awake()
        {
            base.Awake();
        
            combatManager = GetComponent<CombatManager>();

            selectedTargets = new List<GameObject>();

            EventManager.AddListener<CombatStartedEvent>(OnCombatStart);

            EventManager.AddListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);
            EventManager.AddListener<CharacterHoveredEvent>(OnCharacterHovered);
        }

        protected override void OnDestroy()
        {
            EventManager.RemoveListener<CombatStartedEvent>(OnCombatStart);

            EventManager.RemoveListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);
            EventManager.RemoveListener<CharacterHoveredEvent>(OnCharacterHovered);

            base.OnDestroy();
        }

        public void ChooseMove(AbstractCombatMove move, GameObject caster)
        {
            // BUG: left click confirms, but left click also selects a move | trying to select a move after you already selected a move simultaneously confirms the target and then selects the move (which causes you to select a move on someone who is not allowed yet to select a move)

            toBeConfirmedMove = move;
            casterToBe = caster;
        }


        private void OnCharacterHovered(CharacterHoveredEvent @event)
        {
            if (!@event.Character) currentSelectedCharacterIndicator.transform.position = Vector3.zero;
            
            if (!characterList.Contains(@event.Character)) return;
            
            selectedTargets.Add(@event.Character.gameObject);

            currentSelectedCharacterIndicator.transform.position = @event.Character.gameObject.transform.position;
            currentSelectedCharacterIndicator.transform.Translate(Vector3.up * 1.5f);
        }
        

        public void OnTargetSelectConfirm()
        {
            if (selectedTargets.Count == 0) return;
            var confirmedMoveHolder = casterToBe.GetComponent<ConfirmedMoveHolder>();
            confirmedMoveHolder.SelectMove(toBeConfirmedMove, selectedTargets);
            toBeConfirmedMove = null;
            targetsChoosen = false;
            
            selectedTargets.Clear();
        }

        private void OnCombatStart(CombatStartedEvent @event)
        {
            characterList.Clear();

            foreach (var eventEnemy in @event.Enemies) characterList.Add(eventEnemy);

            foreach (var partyMember in PlayerPartySingleton.Instance.Party) characterList.Add(partyMember);
        }

        private void OnCharacterEnterCombat(CharacterEnterCombatEvent @event)
        {
            if (characterList.Contains(@event.Character)) return;

            characterList.Add(@event.Character);
        }
    }
}