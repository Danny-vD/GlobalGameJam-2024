using CombatSystem.Enums;
using CombatSystem.Interfaces;
using CombatSystem.ScriptableAssets.CombatMoves;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class AIChoosingState : AbstractCharacterState
	{
		private CombatMoveManager combatMoveManager;
		private IAIMoveset aiMoveset;
		
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Casting;

		private void Awake()
		{
			combatMoveManager = GetComponent<CombatMoveManager>();
			aiMoveset         = GetComponent<IAIMoveset>();
		}

		public override void Enter()
		{
			CombatMove combatMove = aiMoveset.ChooseAIMove();
			combatMoveManager.SelectMove(combatMove);
			Exit();
		}

		public override void Step()
		{
		}
	}
}