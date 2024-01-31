using System.Linq;
using CharacterScripts;
using CombatMoves.BaseClasses;
using CombatSystem.Enums;
using CombatSystem.Interfaces;
using PlayerPartyScripts;
using VDFramework.Extensions;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class AIChoosingState : AbstractCharacterState
	{
		private SelectedMoveHolder selectedMoveHolder;
		private IAIMoveset aiMoveset;

		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

		private void Awake()
		{
			selectedMoveHolder = GetComponent<SelectedMoveHolder>();
			aiMoveset          = GetComponent<IAIMoveset>();
		}

		public override void Enter()
		{
			AbstractCombatMove combatMove = aiMoveset.ChooseAIMove();
			
			//HACK: taking a random party member does not work if the opposing team is not a valid target (opposing team from enemy is the party)
			selectedMoveHolder.SelectMove(combatMove, PlayerPartySingleton.Instance.Party.Where(obj => !obj.GetComponent<CharacterHealth>().IsDead).GetRandomElement()); //TODO: make better
			Exit();
		}

		public override void Step()
		{
		}
	}
}