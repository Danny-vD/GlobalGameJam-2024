using CombatSystem.Enums;
using CombatSystem.Interfaces;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class AIChoosingState : AbstractCharacterState
	{
		private CombatMoveManager combatMoveManager;
		private IAIMoveset aiMoveset;
		
		public override CharacterStateType NextState => CharacterStateType.Casting;

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