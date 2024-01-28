using CombatSystem.Enums;
using CombatSystem.Interfaces;
using CombatSystem.ScriptableAssets.CombatMoves;

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
			CombatMove combatMove = aiMoveset.ChooseAIMove();
			selectedMoveHolder.SelectMove(combatMove);
			Exit();
		}

		public override void Step()
		{
		}
	}
}