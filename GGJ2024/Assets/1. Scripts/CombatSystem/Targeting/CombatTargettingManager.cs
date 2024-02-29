using System.Collections.Generic;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.CharacterScripts;
using CombatSystem.Events.CharacterSelection;
using CombatSystem.Managers;
using UnityEngine;
using VDFramework.EventSystem;
using VDFramework.Singleton;

namespace CombatSystem.Targeting
{
	public class CombatTargettingManager : Singleton<CombatTargettingManager> // TODO: override target selection when taunted?? (Can a player be taunted?)
	{
		[SerializeField]
		private GameObject currentSelectedCharacterIndicator;

		private GameObject casterToBe;

		private readonly List<GameObject> characterList = new List<GameObject>();

		private CombatManager combatManager;

		private bool targetsChoosen;

		private AbstractCombatMove toBeConfirmedMove;
		private List<GameObject> selectedTargets;

		protected override void Awake()
		{
			base.Awake();

			combatManager = GetComponent<CombatManager>();

			selectedTargets = new List<GameObject>();
			EventManager.AddListener<CharacterClickedEvent>(OnCharacterClicked);
		}

		protected override void OnDestroy()
		{
			EventManager.RemoveListener<CharacterClickedEvent>(OnCharacterClicked);

			base.OnDestroy();
		}

		public void ChooseMove(AbstractCombatMove move, GameObject caster)
		{
			// BUG: left click confirms, but left click also selects a move | trying to select a move after you already selected a move simultaneously confirms the target and then selects the move (which causes you to select a move on someone who is not allowed yet to select a move)

			toBeConfirmedMove = move;
			casterToBe        = caster;
		}


		private void OnCharacterClicked(CharacterClickedEvent @event)
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

			ConfirmedMoveHolder confirmedMoveHolder = casterToBe.GetComponent<ConfirmedMoveHolder>();
			confirmedMoveHolder.SelectMove(toBeConfirmedMove, selectedTargets);
			toBeConfirmedMove = null;
			targetsChoosen    = false;

			selectedTargets.Clear();
		}
	}
}