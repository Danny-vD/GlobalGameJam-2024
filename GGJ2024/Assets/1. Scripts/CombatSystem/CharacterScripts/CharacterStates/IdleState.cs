using CharacterScripts;
using CombatSystem.Enums;
using UnityEngine;

namespace CombatSystem.CharacterScripts.CharacterStates
{
	[RequireComponent(typeof(Character))]
	public class IdleState : AbstractCharacterState
	{
		[SerializeField, Tooltip("The time in seconds that the character has to wait")]
		private float maximumIdleTime = 10;
		
		public override CharacterCombatStateType NextState => CharacterCombatStateType.Choosing;
		public float NormalizedTimer => timer / maximumIdleTime;

		private float timer;

		private Character character;

		private void Awake()
		{
			character = GetComponent<Character>();
		}

		public override void Enter()
		{
			timer = maximumIdleTime;
		}

		public override void Step()
		{
			timer -= character.Statistics.Speed * Time.deltaTime;

			if (timer <= 0)
			{
				Exit();
			}
		}
	}
}