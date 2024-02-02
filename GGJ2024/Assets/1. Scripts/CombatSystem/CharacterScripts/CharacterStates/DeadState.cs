using CharacterScripts;
using CombatSystem.Enums;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	public class DeadState : AbstractCharacterState
	{
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Idle;
		
		private CharacterHealth characterHealth;

		private void Awake()
		{
			characterHealth = GetComponent<CharacterHealth>();

			characterHealth.OnResurrected += Exit;
		}

		private void OnDestroy()
		{
			characterHealth.OnResurrected -= Exit;
		}

		public override void Enter()
		{
		}
	}
}