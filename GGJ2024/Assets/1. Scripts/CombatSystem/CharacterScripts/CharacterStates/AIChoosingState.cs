using CombatMoves.BaseClasses;
using CombatSystem.Enums;
using CombatSystem.Interfaces;

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
			aiMoveset         = GetComponent<IAIMoveset>();
		}

		public override void Enter()
		{
			AbstractCombatMove combatMoveData = aiMoveset.ChooseAIMove();
			selectedMoveHolder.SelectMove(combatMoveData);
			Exit();
		}

		public override void Step()
		{
		}
	}
}