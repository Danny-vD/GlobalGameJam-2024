using System.Collections.Generic;
using UnityEngine;
using VDFramework.EventSystem;

namespace CombatSystem.Events
{
	/// <summary>
	/// This event triggers when any combat starts
	/// </summary>
	public class CombatStartedEvent : VDEvent<CombatStartedEvent>
	{
		public readonly List<GameObject> Enemies;

		public CombatStartedEvent(List<GameObject> enemies)
		{
			Enemies = enemies;
		}
	}
}