using CombatSystem.Events;
using VDFramework;
using VDFramework.EventSystem;

namespace CombatSystem.Managers
{
	public class CombatManager : BetterMonoBehaviour
	{
		public void StartCombat()
		{
			EventManager.RaiseEvent(new CombatStartedEvent());
		}

		public void EndCombat()
		{
			EventManager.RaiseEvent(new CombatEndedEvent());
		}
	}
}