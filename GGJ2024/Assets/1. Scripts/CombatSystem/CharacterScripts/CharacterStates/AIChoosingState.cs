using System.Collections.Generic;
using System.Linq;
using AI;
using CharacterScripts;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Enums;
using CombatSystem.Interfaces;
using PlayerPartyScripts;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class AIChoosingState : AbstractCharacterState
	{
		private ConfirmedMoveHolder confirmedMoveHolder;
		private IAIMoveset aiMoveset;
		
		private AbstractAITargetingLogic aiTargetingLogic;

		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

		private void Awake()
		{
			confirmedMoveHolder = GetComponent<ConfirmedMoveHolder>();
			aiMoveset           = GetComponent<IAIMoveset>();

			aiTargetingLogic = GetComponent<AbstractAITargetingLogic>();
			
			confirmedMoveHolder.OnMoveSelected += Exit;
		}

		public override void Enter()
		{
		}

		public override void Step()
		{
			// NOTE: We can add a small time before this to mimic the AI 'thinking'
			AbstractCombatMove combatMove = aiMoveset.ChooseAIMove();

			// TODO: Override target selection when taunted
			List<GameObject> targets = aiTargetingLogic.GetTargets(combatMove);

			if (targets.Count == 0)
			{
				Debug.LogError("Killed entire party!");
				GetComponent<CharacterStateManager>().ForceState(CharacterCombatStateType.Idle); // TODO lose on TPK
			}
			else
			{
				confirmedMoveHolder.SelectMove(combatMove, targets);
			}
		}

		private void OnDestroy()
		{
			confirmedMoveHolder.OnMoveSelected -= Exit;
		}
	}
}