using System;
using CombatSystem.Events.CharacterSelection;
using CombatSystem.Structs;
using UnityEngine;
using VDFramework;
using VDFramework.EventSystem;

namespace CharacterScripts
{
	public class Character : BetterMonoBehaviour
	{
		[field: SerializeField]
		public CharacterStatistics Statistics { get; private set; }

		public int CurrentMP { get; private set; }

		private void OnMouseEnter()
		{
			EventManager.RaiseEvent(new CharacterHoveredEvent(gameObject));
		}

		private void OnMouseExit()
		{
			//
		}
	}
}