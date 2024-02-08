using System.Collections.Generic;
using System.Linq;
using CharacterScripts;
using CombatMoves.ScriptableObjects.BaseClasses;
using CombatSystem.Enums;
using CombatSystem.Interfaces;
using PlayerPartyScripts;
using UnityEngine;
using VDFramework.Extensions;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class AIChoosingState : AbstractCharacterState
	{
		private ConfirmedMoveHolder confirmedMoveHolder;
		private IAIMoveset aiMoveset;

		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

		private void Awake()
		{
			confirmedMoveHolder = GetComponent<ConfirmedMoveHolder>();
			aiMoveset           = GetComponent<IAIMoveset>();
		}

		// TODO: Override target selection when taunted
		public override void Enter()
		{
			// NOTE: We can add a small time before this to mimic the AI 'thinking'
			AbstractCombatMove combatMove = aiMoveset.ChooseAIMove();

			//HACK: taking a random party member does not work if the opposing team is not a valid target (opposing team from enemy is the party)
			//TODO: Use a separate 'AiTargetingLogic' script that can then differ per AI to make them smarter/dumber with their moves (e.g. one enemy is always random but another targets the lowest HP party member)
			IEnumerable<GameObject> party = PlayerPartySingleton.Instance.Party.Where(obj => !obj.GetComponent<CharacterHealth>().IsDead);

			if (!party.Any()) // TODO: TEMP
			{
				Debug.LogError("Killed entire party!");
			}
			else
			{
				confirmedMoveHolder.SelectMove(combatMove, party.GetRandomElement()); //TODO: Don't take random party member
			}

			Exit();
		}
	}
}