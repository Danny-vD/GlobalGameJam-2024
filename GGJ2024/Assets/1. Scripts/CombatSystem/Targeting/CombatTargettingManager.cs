using System.Collections.Generic;
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

        private GameObject currentSelectedCharacter;

        private bool targetsChoosen;

        private AbstractCombatMove toBeConfirmedMove;

        protected override void Awake()
        {
            base.Awake();

            combatManager = GetComponent<CombatManager>();

            EventManager.AddListener<CombatStartedEvent>(OnCombatStart);
            EventManager.AddListener<CombatEndedEvent>(StopListeneningToConfirmInput);

            EventManager.AddListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);
            EventManager.AddListener<CharacterHoveredEvent>(OnCharacterHovered);
        }

        protected override void OnDestroy()
        {
            EventManager.RemoveListener<CombatStartedEvent>(OnCombatStart);
            EventManager.RemoveListener<CombatEndedEvent>(StopListeneningToConfirmInput);

            EventManager.RemoveListener<CharacterEnterCombatEvent>(OnCharacterEnterCombat);
            EventManager.RemoveListener<CharacterHoveredEvent>(OnCharacterHovered);

            base.OnDestroy();
        }

        public void ChooseMove(AbstractCombatMove move, GameObject caster)
        {
            // BUG: left click confirms, but left click also selects a move | trying to select a move after you already selected a move simultaneously confirms the target and then selects the move (which causes you to select a move on someone who is not allowed yet to select a move)
            StartListeneningToConfirmInput();

            toBeConfirmedMove = move;
            casterToBe = caster;
        }


        private void OnCharacterHovered(CharacterHoveredEvent @event)
        {
            if (!@event.Character) currentSelectedCharacterIndicator.transform.position = Vector3.zero;

            if (!characterList.Contains(@event.Character)) return;

            currentSelectedCharacter = @event.Character.gameObject;

            currentSelectedCharacterIndicator.transform.position = currentSelectedCharacter.transform.position;
            currentSelectedCharacterIndicator.transform.Translate(Vector3.up * 1.5f);
        }

        private void OnTargetSelectConfirm(InputAction.CallbackContext obj)
        {
            if (!toBeConfirmedMove) return;

            //TODO: Check if there is a target selected

            if (targetsChoosen)
            {
                if (toBeConfirmedMove.IsValidTarget(currentSelectedCharacter, casterToBe))
                {
                    var confirmedMoveHolder = casterToBe.GetComponent<ConfirmedMoveHolder>();
                    confirmedMoveHolder.SelectMove(toBeConfirmedMove, currentSelectedCharacter);
                    toBeConfirmedMove = null;
                    targetsChoosen = false;
                    StopListeneningToConfirmInput();
                }
            }
            else
            {
                targetsChoosen = true;
            }
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

        private void StartListeneningToConfirmInput()
        {
            InputControlManager.Instance.playerControls.Combat.MoveConfirm.performed += OnTargetSelectConfirm;
        }

        private void StopListeneningToConfirmInput()
        {
            InputControlManager.Instance.playerControls.Combat.MoveConfirm.performed -= OnTargetSelectConfirm;
        }
    }
}